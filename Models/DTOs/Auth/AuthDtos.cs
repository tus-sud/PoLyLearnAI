using System.ComponentModel.DataAnnotations;
using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Models.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên hiển thị là bắt buộc")]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    public int? PreferredLanguageId { get; set; }
}

public class LoginRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string Password { get; set; } = string.Empty;
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? PreferredLanguageId { get; set; }
    public string? PreferredLanguageCode { get; set; }
    public string? PreferredLanguageName { get; set; }
    public string AvatarKey { get; set; } = "owl";
    public string CompanionKey { get; set; } = "auto";
    public string? Bio { get; set; }
    public int DailyGoalXp { get; set; } = 50;
    public int ExperiencePoints { get; set; }
    public int TodayXp { get; set; }
    public int Level { get; set; } = 1;
    public int CurrentLevelXp { get; set; }
    public int NextLevelXp { get; set; } = 100;
    public string RankTitle { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public UserDto User { get; set; } = null!;
}

public class BadgeDto
{
    public string Code { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsUnlocked { get; set; }
    public string ProgressText { get; set; } = string.Empty;
}

public class UserProfileResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? PreferredLanguageId { get; set; }
    public string? PreferredLanguageCode { get; set; }
    public string? PreferredLanguageName { get; set; }
    public string AvatarKey { get; set; } = "owl";
    public string CompanionKey { get; set; } = "auto";
    public string? Bio { get; set; }
    public int DailyGoalXp { get; set; } = 50;
    public int ExperiencePoints { get; set; }
    public int TodayXp { get; set; }
    public int Level { get; set; } = 1;
    public int CurrentLevelXp { get; set; }
    public int NextLevelXp { get; set; } = 100;
    public string RankTitle { get; set; } = string.Empty;
    public int TotalWordsLearned { get; set; }
    public int TotalWordsMastered { get; set; }
    public int TotalFavoriteWords { get; set; }
    public int TotalLessonsCompleted { get; set; }
    public int TotalQuizzesCompleted { get; set; }
    public int CurrentStreakDays { get; set; }
    public IReadOnlyList<string> ActiveDatesInLastTwoWeeks { get; set; } = [];
    public IReadOnlyList<BadgeDto> Badges { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class UpdateProfileRequest
{
    [Required(ErrorMessage = "Tên hiển thị là bắt buộc")]
    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    public int? PreferredLanguageId { get; set; }

    [MaxLength(50)]
    public string? AvatarKey { get; set; }

    [MaxLength(50)]
    public string? CompanionKey { get; set; }

    [MaxLength(300)]
    public string? Bio { get; set; }

    [Range(10, 500, ErrorMessage = "Mục tiêu XP mỗi ngày phải từ 10 đến 500")]
    public int? DailyGoalXp { get; set; }
}

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự")]
    [MaxLength(100)]
    public string NewPassword { get; set; } = string.Empty;
}
