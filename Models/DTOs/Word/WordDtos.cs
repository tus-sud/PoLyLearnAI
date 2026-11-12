using PolyLearnAI.Api.Models.DTOs.Common;

namespace PolyLearnAI.Api.Models.DTOs.Word;

public class WordFilterParams : PaginationParams
{
    public int? LanguageId { get; set; }
    public string? LanguageCode { get; set; }
    public int? LevelId { get; set; }
    public int? TopicId { get; set; }
    public string? Search { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Sort { get; set; } // "level" (default), "newest", "random"
}

public class HarvestWordsRequest
{
    public int LanguageId { get; set; } = 1;
    public int? LevelId { get; set; }
    public int Count { get; set; } = 8;
    public string? TopicHint { get; set; }
}

public class HarvestWordsResponse
{
    public string Message { get; set; } = string.Empty;
    public int InsertedCount { get; set; }
    public int TotalWordsInLanguage { get; set; }
    public string Source { get; set; } = string.Empty;
    public IReadOnlyList<WordDto> Words { get; set; } = [];
    public int XpEarned { get; set; }
    public int TotalXp { get; set; }
    public int TodayXp { get; set; }
    public int DailyGoalXp { get; set; }
    public int Level { get; set; }
    public bool LeveledUp { get; set; }
}

public class WordDto
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public int LevelId { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public int? TopicId { get; set; }
    public string? TopicName { get; set; }
    public string Term { get; set; } = string.Empty;
    public string? Reading { get; set; }
    public string Meaning { get; set; } = string.Empty;
    public string? PartOfSpeech { get; set; }
    public int ExamplesCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WordExampleDto
{
    public int Id { get; set; }
    public int WordId { get; set; }
    public string Sentence { get; set; } = string.Empty;
    public string? Reading { get; set; }
    public string Translation { get; set; } = string.Empty;
}

public class WordDetailDto : WordDto
{
    public IReadOnlyList<WordExampleDto> Examples { get; set; } = [];
    public bool? IsFavorite { get; set; }
}
