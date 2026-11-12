namespace PolyLearnAI.Api.Models.Entities;

public enum UserRole { User = 0, Admin = 1 }

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    /// <summary>Hash PBKDF2/BCrypt — tuyệt đối không lưu mật khẩu thuần.</summary>
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
    public int? PreferredLanguageId { get; set; }
    public Language? PreferredLanguage { get; set; }
    public string AvatarKey { get; set; } = "owl";
    public string CompanionKey { get; set; } = "auto";
    public string? Bio { get; set; } = "Mỗi ngày tiến bộ 1% cùng PolyLearn AI!";
    public int DailyGoalXp { get; set; } = 50;
    public int ExperiencePoints { get; set; } = 0;
    public int TodayXp { get; set; } = 0;
    public DateTime? LastXpDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    public ICollection<FavoriteWord> Favorites { get; set; } = [];
    public ICollection<UserWordProgress> WordProgress { get; set; } = [];
    public ICollection<LessonProgress> LessonProgress { get; set; } = [];
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = [];
}
