using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Services;

public record XpAwardResult(
    int XpEarned,
    int TotalXp,
    int TodayXp,
    int DailyGoalXp,
    int Level,
    int CurrentLevelXp,
    int NextLevelXp,
    bool LeveledUp,
    string RankTitle);

public static class GamificationHelper
{
    public const int XpPerLevel = 100;

    public static int GetLevel(int totalXp) => Math.Max(1, (Math.Max(0, totalXp) / XpPerLevel) + 1);

    public static int GetCurrentLevelXp(int totalXp) => Math.Max(0, totalXp) % XpPerLevel;

    public static int GetEffectiveTodayXp(User user)
    {
        var today = DateTime.UtcNow.Date;
        return user.LastXpDate.HasValue && user.LastXpDate.Value.Date == today
            ? user.TodayXp
            : 0;
    }

    public static string GetRankTitle(int level) => level switch
    {
        <= 2 => "🌱 Tân Binh Ngôn Ngữ",
        <= 4 => "📘 Học Giả Chăm Chỉ",
        <= 7 => "⚡ Cao Thủ Phản Xạ",
        <= 11 => "🔥 Chiến Binh Đa Ngôn Ngữ",
        _ => "👑 Huyền Thoại Polyglot"
    };

    public static XpAwardResult AwardXp(User user, int amount)
    {
        var safeAmount = Math.Max(0, amount);
        var today = DateTime.UtcNow.Date;
        var oldLevel = GetLevel(user.ExperiencePoints);

        if (user.LastXpDate.HasValue && user.LastXpDate.Value.Date == today)
        {
            user.TodayXp += safeAmount;
        }
        else
        {
            user.TodayXp = safeAmount;
            user.LastXpDate = today;
        }

        user.ExperiencePoints += safeAmount;
        var newLevel = GetLevel(user.ExperiencePoints);

        return new XpAwardResult(
            XpEarned: safeAmount,
            TotalXp: user.ExperiencePoints,
            TodayXp: user.TodayXp,
            DailyGoalXp: user.DailyGoalXp,
            Level: newLevel,
            CurrentLevelXp: GetCurrentLevelXp(user.ExperiencePoints),
            NextLevelXp: XpPerLevel,
            LeveledUp: newLevel > oldLevel,
            RankTitle: GetRankTitle(newLevel)
        );
    }
}

