using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Auth;
using PolyLearnAI.Api.Models.Entities;
using PolyLearnAI.Api.Services.Interfaces;

namespace PolyLearnAI.Api.Services.Implementations;

public class AuthService(
    AppDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IAuthService
{
    private static readonly HashSet<string> ValidAvatarKeys =
        ["owl", "panda", "neko", "tiger", "fox", "dragon", "shiba", "astronaut", "ghost", "wizard", "ninja", "crown", "custom"];

    private static readonly HashSet<string> ValidCompanionKeys =
        ["auto", "owl", "panda", "neko", "tiger", "fox", "dragon"];

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var exists = await dbContext.Users.AnyAsync(u => u.Email == normalizedEmail, ct);
        if (exists)
        {
            throw new BadHttpRequestException("Email này đã được sử dụng.");
        }

        if (request.PreferredLanguageId.HasValue)
        {
            var langExists = await dbContext.Languages.AnyAsync(l => l.Id == request.PreferredLanguageId.Value, ct);
            if (!langExists)
            {
                throw new BadHttpRequestException("Ngôn ngữ ưu tiên không tồn tại trong hệ thống.");
            }
        }

        var defaultAvatar = request.PreferredLanguageId switch
        {
            2 => "panda",
            3 => "neko",
            4 => "tiger",
            _ => "owl"
        };

        var user = new User
        {
            Email = normalizedEmail,
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = passwordHasher.HashPassword(request.Password),
            Role = UserRole.User,
            IsActive = true,
            PreferredLanguageId = request.PreferredLanguageId,
            AvatarKey = defaultAvatar,
            CompanionKey = "auto",
            Bio = "Mỗi ngày tiến bộ 1% cùng PolyLearn AI!",
            DailyGoalXp = 50,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        GamificationHelper.AwardXp(user, 10);

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        // Load navigation property nếu có
        if (user.PreferredLanguageId.HasValue)
        {
            await dbContext.Entry(user).Reference(u => u.PreferredLanguage).LoadAsync(ct);
        }

        var (token, expiresAt) = jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAt,
            User = MapUserDto(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var user = await dbContext.Users
            .Include(u => u.PreferredLanguage)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);

        if (user == null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new BadHttpRequestException("Email hoặc mật khẩu không chính xác.");
        }

        if (!user.IsActive)
        {
            throw new BadHttpRequestException("Tài khoản đã bị tạm khóa. Vui lòng liên hệ quản trị viên.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(ct);

        var (token, expiresAt) = jwtTokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAt,
            User = MapUserDto(user)
        };
    }

    public async Task<UserProfileResponse> GetProfileAsync(int userId, CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.PreferredLanguage)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy người dùng.");
        }

        return await BuildUserProfileAsync(user, ct);
    }

    public async Task<UserProfileResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .Include(u => u.PreferredLanguage)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy người dùng.");
        }

        if (request.PreferredLanguageId.HasValue)
        {
            var langExists = await dbContext.Languages.AnyAsync(l => l.Id == request.PreferredLanguageId.Value, ct);
            if (!langExists)
            {
                throw new BadHttpRequestException("Ngôn ngữ ưu tiên không tồn tại trong hệ thống.");
            }
        }

        user.DisplayName = request.DisplayName.Trim();
        user.PreferredLanguageId = request.PreferredLanguageId;

        if (!string.IsNullOrWhiteSpace(request.AvatarKey))
        {
            var avatar = request.AvatarKey.Trim().ToLowerInvariant();
            if (ValidAvatarKeys.Contains(avatar))
            {
                user.AvatarKey = avatar;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.CompanionKey))
        {
            var companion = request.CompanionKey.Trim().ToLowerInvariant();
            if (ValidCompanionKeys.Contains(companion))
            {
                user.CompanionKey = companion;
            }
        }

        if (request.Bio != null)
        {
            user.Bio = request.Bio.Trim();
        }

        if (request.DailyGoalXp.HasValue)
        {
            user.DailyGoalXp = Math.Clamp(request.DailyGoalXp.Value, 10, 500);
        }

        await dbContext.SaveChangesAsync(ct);

        if (user.PreferredLanguageId.HasValue)
        {
            await dbContext.Entry(user).Reference(u => u.PreferredLanguage).LoadAsync(ct);
        }
        else
        {
            user.PreferredLanguage = null;
        }

        return await BuildUserProfileAsync(user, ct);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy người dùng.");
        }

        if (!passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new BadHttpRequestException("Mật khẩu hiện tại không chính xác.");
        }

        user.PasswordHash = passwordHasher.HashPassword(request.NewPassword);
        await dbContext.SaveChangesAsync(ct);
    }

    private async Task<UserProfileResponse> BuildUserProfileAsync(User user, CancellationToken ct)
    {
        var wordProgress = await dbContext.UserWordProgress
            .AsNoTracking()
            .Where(p => p.UserId == user.Id)
            .Select(p => new { p.Status, p.LastReviewedAt })
            .ToListAsync(ct);

        var totalWordsLearned = wordProgress.Count;
        var totalWordsMastered = wordProgress.Count(p => p.Status == WordStatus.Mastered);

        var totalFavs = await dbContext.FavoriteWords
            .AsNoTracking()
            .CountAsync(f => f.UserId == user.Id, ct);

        var lessonDates = await dbContext.LessonProgress
            .AsNoTracking()
            .Where(lp => lp.UserId == user.Id && lp.IsCompleted)
            .Select(lp => lp.CompletedAt)
            .ToListAsync(ct);

        var totalLessonsCompleted = lessonDates.Count;

        var quizData = await dbContext.QuizAttempts
            .AsNoTracking()
            .Where(q => q.UserId == user.Id && q.CompletedAt.HasValue)
            .Select(q => new { q.Score, q.TotalQuestions, q.CompletedAt })
            .ToListAsync(ct);

        var totalQuizzesCompleted = quizData.Count;
        var passedQuizzes = quizData.Count(q => q.TotalQuestions > 0 && (double)q.Score / q.TotalQuestions >= 0.7);

        var activityDates = new HashSet<DateOnly>();
        foreach (var p in wordProgress.Where(p => p.LastReviewedAt.HasValue))
            activityDates.Add(DateOnly.FromDateTime(p.LastReviewedAt!.Value));
        foreach (var d in lessonDates.Where(d => d.HasValue))
            activityDates.Add(DateOnly.FromDateTime(d!.Value));
        foreach (var q in quizData.Where(q => q.CompletedAt.HasValue))
            activityDates.Add(DateOnly.FromDateTime(q.CompletedAt!.Value));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int streak = 0;
        var checkDate = activityDates.Contains(today) ? today : today.AddDays(-1);
        while (activityDates.Contains(checkDate))
        {
            streak++;
            checkDate = checkDate.AddDays(-1);
        }

        var twoWeeksAgo = today.AddDays(-13);
        var activeDatesList = activityDates
            .Where(d => d >= twoWeeksAgo && d <= today)
            .OrderByDescending(d => d)
            .Select(d => d.ToString("yyyy-MM-dd"))
            .ToList();

        var level = GamificationHelper.GetLevel(user.ExperiencePoints);
        var currentLevelXp = GamificationHelper.GetCurrentLevelXp(user.ExperiencePoints);
        var todayXp = GamificationHelper.GetEffectiveTodayXp(user);

        var badges = new List<BadgeDto>
        {
            new()
            {
                Code = "first_step",
                Icon = "🌱",
                Name = "Khởi Hành Đầy Hứng Khởi",
                Description = "Bắt đầu hành trình học ngoại ngữ tại PolyLearn AI",
                IsUnlocked = true,
                ProgressText = "Đã đạt"
            },
            new()
            {
                Code = "vocab_explorer",
                Icon = "🃏",
                Name = "Nhà Sưu Tập Từ Vựng",
                Description = "Ôn tập ít nhất 10 thẻ từ vựng Leitner",
                IsUnlocked = totalWordsLearned >= 10,
                ProgressText = $"{Math.Min(totalWordsLearned, 10)} / 10 từ"
            },
            new()
            {
                Code = "memory_master",
                Icon = "🏆",
                Name = "Bậc Thầy Trí Nhớ",
                Description = "Đưa ít nhất 5 từ vựng lên Hộp 5 (Thành thạo)",
                IsUnlocked = totalWordsMastered >= 5,
                ProgressText = $"{Math.Min(totalWordsMastered, 5)} / 5 từ"
            },
            new()
            {
                Code = "bookworm",
                Icon = "📖",
                Name = "Mọt Sách Chăm Chỉ",
                Description = "Hoàn thành ít nhất 3 bài học theo lộ trình",
                IsUnlocked = totalLessonsCompleted >= 3,
                ProgressText = $"{Math.Min(totalLessonsCompleted, 3)} / 3 bài"
            },
            new()
            {
                Code = "quiz_sharpshooter",
                Icon = "🎯",
                Name = "Xạ Thủ Trắc Nghiệm",
                Description = "Vượt qua ít nhất 3 bài thi trắc nghiệm (>= 70%)",
                IsUnlocked = passedQuizzes >= 3,
                ProgressText = $"{Math.Min(passedQuizzes, 3)} / 3 đề"
            },
            new()
            {
                Code = "streak_keeper",
                Icon = "🔥",
                Name = "Ngọn Lửa Bền Bỉ",
                Description = "Duy trì chuỗi học tập liên tục 3 ngày trở lên",
                IsUnlocked = streak >= 3,
                ProgressText = $"{Math.Min(streak, 3)} / 3 ngày"
            },
            new()
            {
                Code = "word_lover",
                Icon = "⭐",
                Name = " Sổ Tay Vàng",
                Description = "Lưu ít nhất 5 từ vựng yêu thích vào sổ tay",
                IsUnlocked = totalFavs >= 5,
                ProgressText = $"{Math.Min(totalFavs, 5)} / 5 từ"
            },
            new()
            {
                Code = "polyglot_star",
                Icon = "👑",
                Name = "Ngôi Sao Polyglot",
                Description = "Tích lũy đạt mốc 500 XP (Cấp độ 6)",
                IsUnlocked = user.ExperiencePoints >= 500,
                ProgressText = $"{Math.Min(user.ExperiencePoints, 500)} / 500 XP"
            }
        };

        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role.ToString(),
            PreferredLanguageId = user.PreferredLanguageId,
            PreferredLanguageCode = user.PreferredLanguage?.Code,
            PreferredLanguageName = user.PreferredLanguage?.Name,
            AvatarKey = string.IsNullOrWhiteSpace(user.AvatarKey) ? "owl" : user.AvatarKey,
            CompanionKey = string.IsNullOrWhiteSpace(user.CompanionKey) ? "auto" : user.CompanionKey,
            Bio = user.Bio,
            DailyGoalXp = user.DailyGoalXp > 0 ? user.DailyGoalXp : 50,
            ExperiencePoints = user.ExperiencePoints,
            TodayXp = todayXp,
            Level = level,
            CurrentLevelXp = currentLevelXp,
            NextLevelXp = GamificationHelper.XpPerLevel,
            RankTitle = GamificationHelper.GetRankTitle(level),
            TotalWordsLearned = totalWordsLearned,
            TotalWordsMastered = totalWordsMastered,
            TotalFavoriteWords = totalFavs,
            TotalLessonsCompleted = totalLessonsCompleted,
            TotalQuizzesCompleted = totalQuizzesCompleted,
            CurrentStreakDays = streak,
            ActiveDatesInLastTwoWeeks = activeDatesList,
            Badges = badges,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }

    private static UserDto MapUserDto(User user)
    {
        var level = GamificationHelper.GetLevel(user.ExperiencePoints);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role.ToString(),
            PreferredLanguageId = user.PreferredLanguageId,
            PreferredLanguageCode = user.PreferredLanguage?.Code,
            PreferredLanguageName = user.PreferredLanguage?.Name,
            AvatarKey = string.IsNullOrWhiteSpace(user.AvatarKey) ? "owl" : user.AvatarKey,
            CompanionKey = string.IsNullOrWhiteSpace(user.CompanionKey) ? "auto" : user.CompanionKey,
            Bio = user.Bio,
            DailyGoalXp = user.DailyGoalXp > 0 ? user.DailyGoalXp : 50,
            ExperiencePoints = user.ExperiencePoints,
            TodayXp = GamificationHelper.GetEffectiveTodayXp(user),
            Level = level,
            CurrentLevelXp = GamificationHelper.GetCurrentLevelXp(user.ExperiencePoints),
            NextLevelXp = GamificationHelper.XpPerLevel,
            RankTitle = GamificationHelper.GetRankTitle(level)
        };
    }
}
