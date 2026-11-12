using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Dashboard;
using PolyLearnAI.Api.Models.DTOs.Quiz;
using PolyLearnAI.Api.Models.Entities;
using PolyLearnAI.Api.Services;

namespace PolyLearnAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController(AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Lấy báo cáo tổng hợp tiến độ học tập toàn diện của người dùng
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(
        [FromQuery] int? languageId,
        CancellationToken ct)
    {
        var userId = GetRequiredUserId();
        var now = DateTime.UtcNow;

        var user = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.PreferredLanguage)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy người dùng.");
        }

        var activeLangId = languageId ?? user.PreferredLanguageId ?? 1;

        var language = await dbContext.Languages
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == activeLangId, ct)
            ?? await dbContext.Languages.AsNoTracking().FirstAsync(ct);

        // 1. Thống kê từ vựng & Flashcard Leitner
        var progressList = await dbContext.UserWordProgress
            .AsNoTracking()
            .Where(p => p.UserId == userId && p.Word.LanguageId == language.Id)
            .Select(p => new { p.Box, p.Status, p.NextReviewAt, p.LastReviewedAt })
            .ToListAsync(ct);

        var boxDistribution = new Dictionary<int, int>();
        for (int i = 0; i <= 5; i++)
        {
            boxDistribution[i] = progressList.Count(p => p.Box == i);
        }

        var favoriteCount = await dbContext.FavoriteWords
            .AsNoTracking()
            .CountAsync(f => f.UserId == userId && f.Word.LanguageId == language.Id, ct);

        var vocabSummary = new VocabSummaryDto
        {
            TotalWordsTracked = progressList.Count,
            MasteredWords = progressList.Count(p => p.Status == WordStatus.Mastered),
            LearningWords = progressList.Count(p => p.Status == WordStatus.Learning),
            DueTodayWords = progressList.Count(p => p.NextReviewAt.HasValue && p.NextReviewAt.Value <= now),
            FavoriteWords = favoriteCount,
            BoxDistribution = boxDistribution
        };

        // 2. Thống kê bài học
        var totalLessons = await dbContext.Lessons
            .AsNoTracking()
            .CountAsync(l => l.LanguageId == language.Id && l.IsPublished, ct);

        var completedLessons = await dbContext.LessonProgress
            .AsNoTracking()
            .CountAsync(lp => lp.UserId == userId && lp.Lesson.LanguageId == language.Id && lp.IsCompleted, ct);

        var lessonSummary = new LessonSummaryDto
        {
            CompletedLessons = completedLessons,
            TotalAvailableLessons = totalLessons
        };

        // 3. Thống kê trắc nghiệm
        var quizAttempts = await dbContext.QuizAttempts
            .AsNoTracking()
            .Include(a => a.Language)
            .Include(a => a.Level)
            .Where(a => a.UserId == userId && a.LanguageId == language.Id && a.CompletedAt.HasValue)
            .OrderByDescending(a => a.CompletedAt)
            .ToListAsync(ct);

        var totalQuizzes = quizAttempts.Count;
        var passedQuizzes = quizAttempts.Count(a => a.TotalQuestions > 0 && ((double)a.Score / a.TotalQuestions) >= 0.7);
        var avgPercent = totalQuizzes > 0
            ? Math.Round(quizAttempts.Average(a => a.TotalQuestions > 0 ? ((double)a.Score / a.TotalQuestions * 100) : 0), 1)
            : 0;

        var recentQuizzes = quizAttempts.Take(5).Select(a => new QuizHistoryDto
        {
            AttemptId = a.Id,
            LanguageId = a.LanguageId,
            LanguageCode = a.Language.Code,
            LevelId = a.LevelId,
            LevelCode = a.Level?.Code,
            Score = a.Score,
            TotalQuestions = a.TotalQuestions,
            StartedAt = a.StartedAt,
            CompletedAt = a.CompletedAt
        }).ToList();

        var quizSummary = new QuizSummaryDto
        {
            TotalAttempts = totalQuizzes,
            PassedAttempts = passedQuizzes,
            AveragePercentage = avgPercent,
            RecentAttempts = recentQuizzes
        };

        // 4. Tính toán Streak ngày học liên tục
        var activityDates = new HashSet<DateOnly>();

        foreach (var p in progressList.Where(p => p.LastReviewedAt.HasValue))
        {
            activityDates.Add(DateOnly.FromDateTime(p.LastReviewedAt!.Value));
        }

        var completedLessonDates = await dbContext.LessonProgress
            .AsNoTracking()
            .Where(lp => lp.UserId == userId && lp.CompletedAt.HasValue)
            .Select(lp => lp.CompletedAt!.Value)
            .ToListAsync(ct);

        foreach (var d in completedLessonDates)
        {
            activityDates.Add(DateOnly.FromDateTime(d));
        }

        foreach (var q in quizAttempts.Where(q => q.CompletedAt.HasValue))
        {
            activityDates.Add(DateOnly.FromDateTime(q.CompletedAt!.Value));
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int streak = 0;
        var checkDate = today;

        if (!activityDates.Contains(checkDate))
        {
            checkDate = today.AddDays(-1);
        }

        while (activityDates.Contains(checkDate))
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }

        var twoWeeksAgo = today.AddDays(-13);
        var recentActiveDates = activityDates
            .Where(d => d >= twoWeeksAgo && d <= today)
            .OrderByDescending(d => d)
            .Select(d => d.ToString("yyyy-MM-dd"))
            .ToList();

        var streakSummary = new LearningStreakDto
        {
            CurrentStreakDays = streak,
            LastActiveDate = activityDates.Count > 0 ? activityDates.Max().ToDateTime(TimeOnly.MinValue) : null,
            ActiveDatesInLastTwoWeeks = recentActiveDates
        };

        var userLevel = GamificationHelper.GetLevel(user.ExperiencePoints);
        var gamificationSummary = new GamificationSummaryDto
        {
            AvatarKey = string.IsNullOrWhiteSpace(user.AvatarKey) ? "owl" : user.AvatarKey,
            CompanionKey = string.IsNullOrWhiteSpace(user.CompanionKey) ? "auto" : user.CompanionKey,
            Bio = user.Bio,
            ExperiencePoints = user.ExperiencePoints,
            TodayXp = GamificationHelper.GetEffectiveTodayXp(user),
            DailyGoalXp = user.DailyGoalXp > 0 ? user.DailyGoalXp : 50,
            Level = userLevel,
            CurrentLevelXp = GamificationHelper.GetCurrentLevelXp(user.ExperiencePoints),
            NextLevelXp = GamificationHelper.XpPerLevel,
            RankTitle = GamificationHelper.GetRankTitle(userLevel)
        };

        return Ok(new DashboardSummaryDto
        {
            UserId = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            ActiveLanguageId = language.Id,
            ActiveLanguageCode = language.Code,
            ActiveLanguageName = language.Name,
            Vocab = vocabSummary,
            Lessons = lessonSummary,
            Quizzes = quizSummary,
            Streak = streakSummary,
            Gamification = gamificationSummary
        });
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
