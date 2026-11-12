namespace PolyLearnAI.Api.Models.DTOs.Language;

public class LanguageDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    public string ProficiencySystem { get; set; } = string.Empty;
    public string SpeechLocale { get; set; } = string.Empty;
    public int LevelCount { get; set; }
    public int TopicCount { get; set; }
}

public class LevelDto
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class TopicDto
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int WordCount { get; set; }
}

public class LanguageDetailDto : LanguageDto
{
    public IReadOnlyList<LevelDto> Levels { get; set; } = [];
    public IReadOnlyList<TopicDto> Topics { get; set; } = [];
}
