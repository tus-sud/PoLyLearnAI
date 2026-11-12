using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Common;
using PolyLearnAI.Api.Models.DTOs.Word;
using PolyLearnAI.Api.Models.Entities;
using PolyLearnAI.Api.Services;
using PolyLearnAI.Api.Services.Implementations;
using PolyLearnAI.Api.Services.Interfaces;

namespace PolyLearnAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WordsController(AppDbContext dbContext, IVocabHarvesterService vocabHarvester) : ControllerBase
{
    private static readonly string[] InvalidMeaningsArray = [.. VocabHarvesterService.InvalidRawMeanings];

    /// <summary>
    /// Tìm kiếm và lọc danh sách từ vựng có phân trang
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<WordDto>>> GetWords([FromQuery] WordFilterParams filter, CancellationToken ct)
    {
        var query = dbContext.Words
            .AsNoTracking()
            .Include(w => w.Language)
            .Include(w => w.Level)
            .Include(w => w.Topic)
            .Where(w =>
                !InvalidMeaningsArray.Contains(w.Meaning) &&
                !EF.Functions.Like(w.Term, "[0-9]%") &&
                !EF.Functions.Like(w.Meaning, "[0-9]%"))
            .AsQueryable();

        if (filter.LanguageId.HasValue)
        {
            query = query.Where(w => w.LanguageId == filter.LanguageId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(filter.LanguageCode))
        {
            var code = filter.LanguageCode.Trim().ToLowerInvariant();
            query = query.Where(w => w.Language.Code == code);
        }

        if (filter.LevelId.HasValue)
        {
            query = query.Where(w => w.LevelId == filter.LevelId.Value);
        }

        if (filter.TopicId.HasValue)
        {
            query = query.Where(w => w.TopicId == filter.TopicId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.PartOfSpeech))
        {
            query = query.Where(w => w.PartOfSpeech == filter.PartOfSpeech.Trim());
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(w =>
                w.Term.Contains(term) ||
                (w.Reading != null && w.Reading.Contains(term)) ||
                w.Meaning.Contains(term));
        }

        var totalCount = await query.CountAsync(ct);

        var sortedQuery = (filter.Sort?.Trim().ToLowerInvariant()) switch
        {
            "newest" => query.OrderByDescending(w => w.Id),
            "random" => query.OrderBy(_ => EF.Functions.Random()),
            _ => query.OrderBy(w => w.Level.SortOrder).ThenBy(w => w.Id)
        };

        var items = await sortedQuery
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(w => new WordDto
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                LanguageCode = w.Language.Code,
                LevelId = w.LevelId,
                LevelCode = w.Level.Code,
                LevelName = w.Level.Name,
                TopicId = w.TopicId,
                TopicName = w.Topic != null ? w.Topic.Name : null,
                Term = w.Term,
                Reading = w.Reading,
                Meaning = w.Meaning,
                PartOfSpeech = w.PartOfSpeech,
                ExamplesCount = w.Examples.Count,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<WordDto>(items, totalCount, filter.PageNumber, filter.PageSize));
    }

    /// <summary>
    /// Lấy chi tiết một từ vựng kèm câu ví dụ
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WordDetailDto>> GetById(int id, CancellationToken ct)
    {
        var word = await dbContext.Words
            .AsNoTracking()
            .Include(w => w.Language)
            .Include(w => w.Level)
            .Include(w => w.Topic)
            .Include(w => w.Examples)
            .FirstOrDefaultAsync(w => w.Id == id, ct);

        if (word == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy từ vựng ID {id}.");
        }

        bool? isFav = null;
        var userId = TryGetCurrentUserId();
        if (userId.HasValue)
        {
            isFav = await dbContext.FavoriteWords
                .AnyAsync(f => f.UserId == userId.Value && f.WordId == id, ct);
        }

        var dto = new WordDetailDto
        {
            Id = word.Id,
            LanguageId = word.LanguageId,
            LanguageCode = word.Language.Code,
            LevelId = word.LevelId,
            LevelCode = word.Level.Code,
            LevelName = word.Level.Name,
            TopicId = word.TopicId,
            TopicName = word.Topic?.Name,
            Term = word.Term,
            Reading = word.Reading,
            Meaning = word.Meaning,
            PartOfSpeech = word.PartOfSpeech,
            ExamplesCount = word.Examples.Count,
            CreatedAt = word.CreatedAt,
            IsFavorite = isFav,
            Examples = word.Examples.Select(ex => new WordExampleDto
            {
                Id = ex.Id,
                WordId = ex.WordId,
                Sentence = ex.Sentence,
                Reading = ex.Reading,
                Translation = ex.Translation
            }).ToList()
        };

        return Ok(dto);
    }

    /// <summary>
    /// Lấy ngẫu nhiên N từ vựng phục vụ luyện tập / Flashcard nhanh
    /// </summary>
    [HttpGet("random")]
    public async Task<ActionResult<IReadOnlyList<WordDto>>> GetRandomWords(
        [FromQuery] int? languageId,
        [FromQuery] int? levelId,
        [FromQuery] int? topicId,
        [FromQuery] int count = 10,
        CancellationToken ct = default)
    {
        count = Math.Clamp(count, 1, 50);

        var query = dbContext.Words
            .AsNoTracking()
            .Include(w => w.Language)
            .Include(w => w.Level)
            .Include(w => w.Topic)
            .Where(w =>
                !InvalidMeaningsArray.Contains(w.Meaning) &&
                !EF.Functions.Like(w.Term, "[0-9]%") &&
                !EF.Functions.Like(w.Meaning, "[0-9]%"))
            .AsQueryable();

        if (languageId.HasValue)
        {
            query = query.Where(w => w.LanguageId == languageId.Value);
        }

        if (levelId.HasValue)
        {
            query = query.Where(w => w.LevelId == levelId.Value);
        }

        if (topicId.HasValue)
        {
            query = query.Where(w => w.TopicId == topicId.Value);
        }

        var items = await query
            .OrderBy(_ => EF.Functions.Random())
            .Take(count)
            .Select(w => new WordDto
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                LanguageCode = w.Language.Code,
                LevelId = w.LevelId,
                LevelCode = w.Level.Code,
                LevelName = w.Level.Name,
                TopicId = w.TopicId,
                TopicName = w.Topic != null ? w.Topic.Name : null,
                Term = w.Term,
                Reading = w.Reading,
                Meaning = w.Meaning,
                PartOfSpeech = w.PartOfSpeech,
                ExamplesCount = w.Examples.Count,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync(ct);

        return Ok(items);
    }

    /// <summary>
    /// Khám phá và lấy thêm từ vựng mới (AI / Từ điển trực tuyến / Kho mở rộng), lưu vĩnh viễn vào Database
    /// </summary>
    [HttpPost("harvest")]
    [Authorize]
    public async Task<ActionResult<HarvestWordsResponse>> HarvestPermanentWords(
        [FromBody] HarvestWordsRequest request,
        CancellationToken ct)
    {
        var userId = GetRequiredUserId();
        var batch = await vocabHarvester.HarvestAndPersistWordsAsync(
            request.LanguageId,
            request.Count,
            request.TopicHint,
            request.LevelId,
            ct);

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        var xpAmount = batch.InsertedCount > 0 ? Math.Min(25, batch.InsertedCount * 2) : 0;
        var xpAward = GamificationHelper.AwardXp(user, xpAmount);
        if (xpAmount > 0)
        {
            await dbContext.SaveChangesAsync(ct);
        }

        var message = batch.InsertedCount > 0
            ? $"🎉 Đã lấy và lưu vĩnh viễn +{batch.InsertedCount} từ vựng mới ({batch.SourceLabel})! Tổng kho hiện có {batch.TotalWordsInLanguage} từ."
            : $"Kho từ hiện đã rất phong phú ({batch.TotalWordsInLanguage} từ). Hãy bấm Tạo đề ngẫu nhiên để luyện tập ngay!";

        return Ok(new HarvestWordsResponse
        {
            Message = message,
            InsertedCount = batch.InsertedCount,
            TotalWordsInLanguage = batch.TotalWordsInLanguage,
            Source = batch.SourceLabel,
            Words = batch.InsertedWords,
            XpEarned = xpAward.XpEarned,
            TotalXp = xpAward.TotalXp,
            TodayXp = xpAward.TodayXp,
            DailyGoalXp = xpAward.DailyGoalXp,
            Level = xpAward.Level,
            LeveledUp = xpAward.LeveledUp
        });
    }

    /// <summary>
    /// Đánh dấu yêu thích từ vựng
    /// </summary>
    [HttpPost("{id:int}/favorite")]
    [Authorize]
    public async Task<IActionResult> AddFavorite(int id, CancellationToken ct)
    {
        var userId = GetRequiredUserId();

        var wordExists = await dbContext.Words.AnyAsync(w => w.Id == id, ct);
        if (!wordExists)
        {
            throw new KeyNotFoundException($"Không tìm thấy từ vựng ID {id}.");
        }

        var alreadyFav = await dbContext.FavoriteWords.AnyAsync(f => f.UserId == userId && f.WordId == id, ct);
        if (!alreadyFav)
        {
            dbContext.FavoriteWords.Add(new FavoriteWord
            {
                UserId = userId,
                WordId = id,
                CreatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync(ct);
        }

        return Ok(new { message = "Đã thêm từ vựng vào danh sách yêu thích." });
    }

    /// <summary>
    /// Xóa từ vựng khỏi danh sách yêu thích
    /// </summary>
    [HttpDelete("{id:int}/favorite")]
    [Authorize]
    public async Task<IActionResult> RemoveFavorite(int id, CancellationToken ct)
    {
        var userId = GetRequiredUserId();

        var favorite = await dbContext.FavoriteWords
            .FirstOrDefaultAsync(f => f.UserId == userId && f.WordId == id, ct);

        if (favorite != null)
        {
            dbContext.FavoriteWords.Remove(favorite);
            await dbContext.SaveChangesAsync(ct);
        }

        return Ok(new { message = "Đã bỏ yêu thích từ vựng." });
    }

    /// <summary>
    /// Lấy danh sách từ vựng yêu thích của người dùng hiện tại
    /// </summary>
    [HttpGet("favorites")]
    [Authorize]
    public async Task<ActionResult<PagedResult<WordDto>>> GetFavorites([FromQuery] PaginationParams pagination, CancellationToken ct)
    {
        var userId = GetRequiredUserId();

        var query = dbContext.FavoriteWords
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => f.Word);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(w => new WordDto
            {
                Id = w.Id,
                LanguageId = w.LanguageId,
                LanguageCode = w.Language.Code,
                LevelId = w.LevelId,
                LevelCode = w.Level.Code,
                LevelName = w.Level.Name,
                TopicId = w.TopicId,
                TopicName = w.Topic != null ? w.Topic.Name : null,
                Term = w.Term,
                Reading = w.Reading,
                Meaning = w.Meaning,
                PartOfSpeech = w.PartOfSpeech,
                ExamplesCount = w.Examples.Count,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<WordDto>(items, totalCount, pagination.PageNumber, pagination.PageSize));
    }

    private int? TryGetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out var userId))
        {
            return userId;
        }
        return null;
    }

    private int GetRequiredUserId()
    {
        return TryGetCurrentUserId() ?? throw new UnauthorizedAccessException("Cần đăng nhập để thực hiện chức năng này.");
    }
}
