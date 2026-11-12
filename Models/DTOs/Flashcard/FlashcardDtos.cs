using System.ComponentModel.DataAnnotations;
using PolyLearnAI.Api.Models.DTOs.Word;
using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Models.DTOs.Flashcard;

public class FlashcardDueFilterParams
{
    [Required]
    public int LanguageId { get; set; }
    public int? LevelId { get; set; }
    public int? TopicId { get; set; }
    public int Limit { get; set; } = 20;
}

public class FlashcardItemDto
{
    public int WordId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public int LevelId { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string? Reading { get; set; }
    public string Meaning { get; set; } = string.Empty;
    public string? PartOfSpeech { get; set; }
    public int Box { get; set; }
    public WordStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public int ReviewCount { get; set; }
    public DateTime? NextReviewAt { get; set; }
    public IReadOnlyList<WordExampleDto> Examples { get; set; } = [];
}

public class ReviewCardRequest
{
    [Required]
    public int WordId { get; set; }

    /// <summary>
    /// true: nhớ được (thăng hạng Box), false: quên (rớt về Box 0)
    /// </summary>
    [Required]
    public bool IsRemembered { get; set; }
}

public class ReviewCardResponse
{
    public int WordId { get; set; }
    public int PreviousBox { get; set; }
    public int NewBox { get; set; }
    public WordStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public int ReviewCount { get; set; }
    public DateTime NextReviewAt { get; set; }
    public string Message { get; set; } = string.Empty;
    public int XpEarned { get; set; }
    public int TotalXp { get; set; }
    public int TodayXp { get; set; }
    public int DailyGoalXp { get; set; }
    public int Level { get; set; }
    public bool LeveledUp { get; set; }
}

public class FlashcardStatsDto
{
    public int TotalTracked { get; set; }
    public int DueCount { get; set; }
    public int MasteredCount { get; set; }
    public int LearningCount { get; set; }
    public Dictionary<int, int> BoxDistribution { get; set; } = [];
}
