namespace PolyLearnAI.Api.Models.Entities;

public class Language
{
    public int Id { get; set; }
    /// <summary>en, zh, ja, ko</summary>
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NativeName { get; set; } = string.Empty;
    /// <summary>CEFR, HSK, JLPT, TOPIK</summary>
    public string ProficiencySystem { get; set; } = string.Empty;
    /// <summary>Locale cho Text-to-Speech, vd en-US, zh-CN, ja-JP, ko-KR</summary>
    public string SpeechLocale { get; set; } = string.Empty;

    public ICollection<Level> Levels { get; set; } = [];
    public ICollection<Topic> Topics { get; set; } = [];
}

public class Level
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
    public string Code { get; set; } = string.Empty;   // A1, HSK1, N5, TOPIK1
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class Topic
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
}

public class Word
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
    public int LevelId { get; set; }
    public Level Level { get; set; } = null!;
    public int? TopicId { get; set; }
    public Topic? Topic { get; set; }

    /// <summary>Từ gốc: 你好, こんにちは, 안녕하세요, Hello</summary>
    public string Term { get; set; } = string.Empty;
    /// <summary>Phiên âm: pinyin, romaji, romanization, IPA</summary>
    public string? Reading { get; set; }
    public string Meaning { get; set; } = string.Empty;
    public string? PartOfSpeech { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<WordExample> Examples { get; set; } = [];
}

public class WordExample
{
    public int Id { get; set; }
    public int WordId { get; set; }
    public Word Word { get; set; } = null!;
    public string Sentence { get; set; } = string.Empty;
    public string? Reading { get; set; }
    public string Translation { get; set; } = string.Empty;
}
