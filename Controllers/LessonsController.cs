using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Common;
using PolyLearnAI.Api.Models.DTOs.Lesson;
using PolyLearnAI.Api.Models.Entities;
using PolyLearnAI.Api.Services;

namespace PolyLearnAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController(AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách các bài học có bộ lọc và phân trang
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<LessonDto>>> GetLessons([FromQuery] LessonFilterParams filter, CancellationToken ct)
    {
        var query = dbContext.Lessons
            .AsNoTracking()
            .Include(l => l.Language)
            .Include(l => l.Level)
            .Where(l => l.IsPublished)
            .AsQueryable();

        if (filter.LanguageId.HasValue)
        {
            query = query.Where(l => l.LanguageId == filter.LanguageId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(filter.LanguageCode))
        {
            var code = filter.LanguageCode.Trim().ToLowerInvariant();
            query = query.Where(l => l.Language.Code == code);
        }

        if (filter.LevelId.HasValue)
        {
            query = query.Where(l => l.LevelId == filter.LevelId.Value);
        }

        if (filter.Type.HasValue)
        {
            query = query.Where(l => l.Type == filter.Type.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(l => l.Title.Contains(term) || (l.Content != null && l.Content.Contains(term)));
        }

        var totalCount = await query.CountAsync(ct);

        var userId = TryGetCurrentUserId();
        List<int> completedLessonIds = [];
        if (userId.HasValue)
        {
            completedLessonIds = await dbContext.LessonProgress
                .Where(lp => lp.UserId == userId.Value && lp.IsCompleted)
                .Select(lp => lp.LessonId)
                .ToListAsync(ct);
        }

        var items = await query
            .OrderBy(l => l.SortOrder)
            .ThenBy(l => l.Id)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(l => new
            {
                l.Id,
                l.LanguageId,
                LanguageCode = l.Language.Code,
                l.LevelId,
                LevelCode = l.Level.Code,
                LevelName = l.Level.Name,
                l.Title,
                l.Type,
                l.Content,
                l.SortOrder,
                QuestionCount = l.Questions.Count,
                l.CreatedAt
            })
            .ToListAsync(ct);

        var dtoList = items.Select(l => new LessonDto
        {
            Id = l.Id,
            LanguageId = l.LanguageId,
            LanguageCode = l.LanguageCode,
            LevelId = l.LevelId,
            LevelCode = l.LevelCode,
            LevelName = l.LevelName,
            Title = l.Title,
            Type = l.Type,
            ContentPreview = l.Content != null && l.Content.Length > 120 ? l.Content[..120] + "..." : l.Content,
            SortOrder = l.SortOrder,
            QuestionCount = l.QuestionCount,
            CreatedAt = l.CreatedAt,
            IsCompleted = userId.HasValue ? completedLessonIds.Contains(l.Id) : null
        }).ToList();

        return Ok(new PagedResult<LessonDto>(dtoList, totalCount, filter.PageNumber, filter.PageSize));
    }

    /// <summary>
    /// Lấy chi tiết bài học kèm nội dung đọc/nghe và bộ câu hỏi luyện tập
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LessonDetailDto>> GetById(int id, CancellationToken ct)
    {
        var lesson = await dbContext.Lessons
            .AsNoTracking()
            .Include(l => l.Language)
            .Include(l => l.Level)
            .Include(l => l.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(l => l.Id == id && l.IsPublished, ct);

        if (lesson == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy bài học ID {id}.");
        }

        var userId = TryGetCurrentUserId();
        bool? isCompleted = null;
        if (userId.HasValue)
        {
            isCompleted = await dbContext.LessonProgress
                .AnyAsync(lp => lp.UserId == userId.Value && lp.LessonId == id && lp.IsCompleted, ct);
        }

        var detail = new LessonDetailDto
        {
            Id = lesson.Id,
            LanguageId = lesson.LanguageId,
            LanguageCode = lesson.Language.Code,
            LevelId = lesson.LevelId,
            LevelCode = lesson.Level.Code,
            LevelName = lesson.Level.Name,
            Title = lesson.Title,
            Type = lesson.Type,
            Content = lesson.Content,
            ContentPreview = lesson.Content != null && lesson.Content.Length > 120 ? lesson.Content[..120] + "..." : lesson.Content,
            SortOrder = lesson.SortOrder,
            QuestionCount = lesson.Questions.Count,
            CreatedAt = lesson.CreatedAt,
            IsCompleted = isCompleted,
            Questions = lesson.Questions.Select(q => new LessonQuestionDto
            {
                Id = q.Id,
                Text = q.Text,
                Explanation = q.Explanation,
                Options = q.Options.Select(o => new LessonQuestionOptionDto
                {
                    Id = o.Id,
                    Text = o.Text,
                    IsCorrect = o.IsCorrect
                }).ToList()
            }).ToList()
        };

        return Ok(detail);
    }

    /// <summary>
    /// Đánh dấu hoàn thành bài học
    /// </summary>
    [HttpPost("{id:int}/complete")]
    [Authorize]
    public async Task<IActionResult> MarkCompleted(int id, CancellationToken ct)
    {
        var userId = GetRequiredUserId();

        var lessonExists = await dbContext.Lessons.AnyAsync(l => l.Id == id && l.IsPublished, ct);
        if (!lessonExists)
        {
            throw new KeyNotFoundException($"Không tìm thấy bài học ID {id}.");
        }

        var progress = await dbContext.LessonProgress
            .FirstOrDefaultAsync(lp => lp.UserId == userId && lp.LessonId == id, ct);

        bool wasAlreadyCompleted = progress?.IsCompleted == true;

        if (progress == null)
        {
            dbContext.LessonProgress.Add(new LessonProgress
            {
                UserId = userId,
                LessonId = id,
                IsCompleted = true,
                CompletedAt = DateTime.UtcNow
            });
        }
        else
        {
            progress.IsCompleted = true;
            progress.CompletedAt = DateTime.UtcNow;
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        var xpAward = GamificationHelper.AwardXp(user, wasAlreadyCompleted ? 5 : 25);

        await dbContext.SaveChangesAsync(ct);
        return Ok(new
        {
            message = "Đã ghi nhận hoàn thành bài học.",
            xpEarned = xpAward.XpEarned,
            totalXp = xpAward.TotalXp,
            todayXp = xpAward.TodayXp,
            dailyGoalXp = xpAward.DailyGoalXp,
            level = xpAward.Level,
            leveledUp = xpAward.LeveledUp
        });
    }

    /// <summary>
    /// Lấy danh sách ID các bài học đã hoàn thành của người dùng
    /// </summary>
    [HttpGet("completed-ids")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<int>>> GetCompletedLessonIds(CancellationToken ct)
    {
        var userId = GetRequiredUserId();
        var ids = await dbContext.LessonProgress
            .AsNoTracking()
            .Where(lp => lp.UserId == userId && lp.IsCompleted)
            .Select(lp => lp.LessonId)
            .ToListAsync(ct);

        return Ok(ids);
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
