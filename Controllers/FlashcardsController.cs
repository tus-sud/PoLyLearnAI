using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Flashcard;
using PolyLearnAI.Api.Models.DTOs.Word;
using PolyLearnAI.Api.Models.Entities;
using PolyLearnAI.Api.Services;
using PolyLearnAI.Api.Services.Interfaces;

namespace PolyLearnAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FlashcardsController(AppDbContext dbContext, IVocabHarvesterService vocabHarvester) : ControllerBase
{
    private static readonly int[] BoxIntervalDays = [1, 2, 4, 7, 14, 30];

    /// <summary>
    /// Lấy danh sách thẻ từ vựng cần ôn tập hôm nay (theo thuật toán Leitner)
    /// </summary>
    [HttpGet("due")]
    public async Task<ActionResult<IReadOnlyList<FlashcardItemDto>>> GetDueCards(
        [FromQuery] FlashcardDueFilterParams filter,
        CancellationToken ct)
    {
        var userId = GetRequiredUserId();
        var now = DateTime.UtcNow;
        var limit = Math.Clamp(filter.Limit, 1, 50);

        // 1. Thẻ đã học và đến hạn ôn (NextReviewAt <= now)
        var dueProgress = await dbContext.UserWordProgress
            .AsNoTracking()
            .Include(p => p.Word)
                .ThenInclude(w => w.Language)
            .Include(p => p.Word)
                .ThenInclude(w => w.Level)
            .Include(p => p.Word)
                .ThenInclude(w => w.Examples)
            .Where(p => p.UserId == userId &&
                        p.Word.LanguageId == filter.LanguageId &&
                        (!filter.LevelId.HasValue || p.Word.LevelId == filter.LevelId.Value) &&
                        (!filter.TopicId.HasValue || p.Word.TopicId == filter.TopicId.Value) &&
                        p.NextReviewAt.HasValue && p.NextReviewAt.Value <= now)
            .OrderBy(p => p.NextReviewAt)
            .Take(limit)
            .ToListAsync(ct);

        var result = dueProgress.Select(p => MapFlashcard(p.Word, p.Box, p.Status, p.ReviewCount, p.NextReviewAt)).ToList();

        // 2. Nếu chưa đủ limit, lấy thêm từ mới chưa học
        if (result.Count < limit)
        {
            var remaining = limit - result.Count;
            var trackedWordIds = await dbContext.UserWordProgress
                .Where(p => p.UserId == userId && p.Word.LanguageId == filter.LanguageId)
                .Select(p => p.WordId)
                .ToListAsync(ct);

            var newWords = await dbContext.Words
                .AsNoTracking()
                .Include(w => w.Language)
                .Include(w => w.Level)
                .Include(w => w.Examples)
                .Where(w => w.LanguageId == filter.LanguageId &&
                            (!filter.LevelId.HasValue || w.LevelId == filter.LevelId.Value) &&
                            (!filter.TopicId.HasValue || w.TopicId == filter.TopicId.Value) &&
                            !trackedWordIds.Contains(w.Id))
                .OrderBy(w => w.Level.SortOrder)
                .ThenBy(w => w.Id)
                .Take(remaining)
                .ToListAsync(ct);

            result.AddRange(newWords.Select(w => MapFlashcard(w, 0, WordStatus.New, 0, null)));
        }

        return Ok(result);
    }

    /// <summary>
    /// Rút bộ thẻ Flashcard ngẫu nhiên không giới hạn (kèm tùy chọn săn thêm từ mới lưu vĩnh viễn vào Database)
    /// </summary>
    [HttpGet("random")]
    public async Task<ActionResult<IReadOnlyList<FlashcardItemDto>>> GetEndlessRandomCards(
        [FromQuery] int languageId = 1,
        [FromQuery] int? levelId = null,
        [FromQuery] int limit = 15,
        [FromQuery] bool harvestNew = false,
        [FromQuery] int newCount = 8,
        [FromQuery] string? topicHint = null,
        CancellationToken ct = default)
    {
        var userId = GetRequiredUserId();
        limit = Math.Clamp(limit, 1, 50);

        IReadOnlyList<int> priorityIds = [];
        if (harvestNew)
        {
            var batch = await vocabHarvester.HarvestAndPersistWordsAsync(
                languageId,
                Math.Clamp(newCount, 1, 20),
                topicHint,
                levelId,
                ct);
            priorityIds = batch.InsertedWordIds;
        }

        var baseQuery = dbContext.Words
            .AsNoTracking()
            .Include(w => w.Language)
            .Include(w => w.Level)
            .Include(w => w.Examples)
            .Where(w => w.LanguageId == languageId && (!levelId.HasValue || w.LevelId == levelId.Value));

        var selectedWords = new List<Word>();
        if (priorityIds.Count > 0)
        {
            var freshWords = await baseQuery
                .Where(w => priorityIds.Contains(w.Id))
                .ToListAsync(ct);
            selectedWords.AddRange(freshWords);
        }

        if (selectedWords.Count < limit)
        {
            var excludeIds = selectedWords.Select(w => w.Id).ToList();
            var randomMore = await baseQuery
                .Where(w => !excludeIds.Contains(w.Id))
                .OrderBy(_ => EF.Functions.Random())
                .Take(limit - selectedWords.Count)
                .ToListAsync(ct);
            selectedWords.AddRange(randomMore);
        }

        var wordIds = selectedWords.Select(w => w.Id).ToList();
        var progressMap = await dbContext.UserWordProgress
            .AsNoTracking()
            .Where(p => p.UserId == userId && wordIds.Contains(p.WordId))
            .ToDictionaryAsync(p => p.WordId, ct);

        var cards = selectedWords.Select(w =>
        {
            if (progressMap.TryGetValue(w.Id, out var p))
            {
                return MapFlashcard(w, p.Box, p.Status, p.ReviewCount, p.NextReviewAt);
            }
            return MapFlashcard(w, 0, WordStatus.New, 0, null);
        }).ToList();

        return Ok(cards);
    }

    /// <summary>
    /// Ghi nhận kết quả đánh giá thẻ (Nhớ / Quên) và nâng/hạ hộp Leitner
    /// </summary>
    [HttpPost("review")]
    public async Task<ActionResult<ReviewCardResponse>> ReviewCard(
        [FromBody] ReviewCardRequest request,
        CancellationToken ct)
    {
        var userId = GetRequiredUserId();

        var wordExists = await dbContext.Words.AnyAsync(w => w.Id == request.WordId, ct);
        if (!wordExists)
        {
            throw new KeyNotFoundException($"Không tìm thấy từ vựng ID {request.WordId}.");
        }

        var progress = await dbContext.UserWordProgress
            .FirstOrDefaultAsync(p => p.UserId == userId && p.WordId == request.WordId, ct);

        int oldBox = progress?.Box ?? 0;
        int newBox;

        if (request.IsRemembered)
        {
            newBox = Math.Min(5, oldBox + 1);
        }
        else
        {
            newBox = 0; // Quay về hộp 0 để củng cố lại
        }

        var status = newBox == 5 ? WordStatus.Mastered : WordStatus.Learning;
        var intervalDays = BoxIntervalDays[newBox];
        var nextReviewAt = DateTime.UtcNow.AddDays(intervalDays);

        if (progress == null)
        {
            progress = new UserWordProgress
            {
                UserId = userId,
                WordId = request.WordId,
                Box = newBox,
                Status = status,
                ReviewCount = 1,
                LastReviewedAt = DateTime.UtcNow,
                NextReviewAt = nextReviewAt
            };
            dbContext.UserWordProgress.Add(progress);
        }
        else
        {
            progress.Box = newBox;
            progress.Status = status;
            progress.ReviewCount += 1;
            progress.LastReviewedAt = DateTime.UtcNow;
            progress.NextReviewAt = nextReviewAt;
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        var xpAmount = request.IsRemembered ? (newBox == 5 ? 20 : 10) : 3;
        var xpAward = GamificationHelper.AwardXp(user, xpAmount);

        await dbContext.SaveChangesAsync(ct);

        var message = request.IsRemembered
            ? (newBox == 5 ? "Tuyệt vời! Bạn đã thành thạo từ vựng này." : $"Rất tốt! Thẻ đã lên Hộp {newBox}, ôn lại sau {intervalDays} ngày.")
            : $"Đã chuyển về Hộp {newBox} để củng cố lại vào ngày mai.";

        return Ok(new ReviewCardResponse
        {
            WordId = request.WordId,
            PreviousBox = oldBox,
            NewBox = newBox,
            Status = status,
            ReviewCount = progress.ReviewCount,
            NextReviewAt = nextReviewAt,
            Message = message,
            XpEarned = xpAward.XpEarned,
            TotalXp = xpAward.TotalXp,
            TodayXp = xpAward.TodayXp,
            DailyGoalXp = xpAward.DailyGoalXp,
            Level = xpAward.Level,
            LeveledUp = xpAward.LeveledUp
        });
    }

    /// <summary>
    /// Thống kê tiến độ flashcard theo thuật toán Leitner
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<FlashcardStatsDto>> GetStats([FromQuery] int languageId, CancellationToken ct)
    {
        var userId = GetRequiredUserId();
        var now = DateTime.UtcNow;

        var userProgressList = await dbContext.UserWordProgress
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.Word.LanguageId == languageId)
            .Select(p => new { p.Box, p.Status, p.NextReviewAt })
            .ToListAsync(ct);

        var boxDistribution = new Dictionary<int, int>();
        for (int i = 0; i <= 5; i++)
        {
            boxDistribution[i] = userProgressList.Count(p => p.Box == i);
        }

        var stats = new FlashcardStatsDto
        {
            TotalTracked = userProgressList.Count,
            DueCount = userProgressList.Count(p => p.NextReviewAt.HasValue && p.NextReviewAt.Value <= now),
            MasteredCount = userProgressList.Count(p => p.Status == WordStatus.Mastered),
            LearningCount = userProgressList.Count(p => p.Status == WordStatus.Learning),
            BoxDistribution = boxDistribution
        };

        return Ok(stats);
    }

    private static FlashcardItemDto MapFlashcard(Word w, int box, WordStatus status, int reviewCount, DateTime? nextReviewAt)
    {
        return new FlashcardItemDto
        {
            WordId = w.Id,
            LanguageId = w.LanguageId,
            LanguageCode = w.Language.Code,
            LevelId = w.LevelId,
            LevelCode = w.Level.Code,
            Term = w.Term,
            Reading = w.Reading,
            Meaning = w.Meaning,
            PartOfSpeech = w.PartOfSpeech,
            Box = box,
            Status = status,
            ReviewCount = reviewCount,
            NextReviewAt = nextReviewAt,
            Examples = w.Examples.Select(ex => new WordExampleDto
            {
                Id = ex.Id,
                WordId = ex.WordId,
                Sentence = ex.Sentence,
                Reading = ex.Reading,
                Translation = ex.Translation
            }).ToList()
        };
    }

    private int GetRequiredUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Cần đăng nhập để sử dụng tính năng này.");
        }
        return userId;
    }
}
