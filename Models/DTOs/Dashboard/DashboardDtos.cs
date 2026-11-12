using PolyLearnAI.Api.Models.DTOs.Quiz;

namespace PolyLearnAI.Api.Models.DTOs.Dashboard;

public class VocabSummaryDto
{
    public int TotalWordsTracked { get; set; }
    public int MasteredWords { get; set; }
    public int LearningWords { get; set; }
    public int DueTodayWords { get; set; }
    public int FavoriteWords { get; set; }
    public Dictionary<int, int> BoxDistribution { get; set; } = [];
}

public class LessonSummaryDto
{
    public int CompletedLessons { get; set; }
    public int TotalAvailableLessons { get; set; }
    public double CompletionPercentage => TotalAvailableLessons > 0
        ? Math.Round((double)CompletedLessons / TotalAvailableLessons * 100, 1)
        : 0;
}

public class QuizSummaryDto
{
    public int TotalAttempts { get; set; }
    public int PassedAttempts { get; set; }
    public double AveragePercentage { get; set; }
    public IReadOnlyList<QuizHistoryDto> RecentAttempts { get; set; } = [];
}

public class LearningStreakDto
{
    public int CurrentStreakDays { get; set; }
    public DateTime? LastActiveDate { get; set; }
    public IReadOnlyList<string> ActiveDatesInLastTwoWeeks { get; set; } = [];
}

public class GamificationSummaryDto
{
    public string AvatarKey { get; set; } = "owl";
    public string CompanionKey { get; set; } = "auto";
    public string? Bio { get; set; }
    public int ExperiencePoints { get; set; }
    public int TodayXp { get; set; }
    public int DailyGoalXp { get; set; } = 50;
    public int Level { get; set; } = 1;
    public int CurrentLevelXp { get; set; }
    public int NextLevelXp { get; set; } = 100;
    public string RankTitle { get; set; } = string.Empty;
}

public class DashboardSummaryDto
{
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int ActiveLanguageId { get; set; }
    public string ActiveLanguageCode { get; set; } = string.Empty;
    public string ActiveLanguageName { get; set; } = string.Empty;

    public VocabSummaryDto Vocab { get; set; } = new();
    public LessonSummaryDto Lessons { get; set; } = new();
    public QuizSummaryDto Quizzes { get; set; } = new();
    public LearningStreakDto Streak { get; set; } = new();
    public GamificationSummaryDto Gamification { get; set; } = new();
}
