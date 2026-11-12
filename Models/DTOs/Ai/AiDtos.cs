using System.ComponentModel.DataAnnotations;

namespace PolyLearnAI.Api.Models.DTOs.Ai;

public class GrammarCheckRequest
{
    [Required(ErrorMessage = "Câu cần kiểm tra là bắt buộc")]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã ngôn ngữ là bắt buộc (en, zh, ja, ko)")]
    public string LanguageCode { get; set; } = "en";

    public string? TargetMeaning { get; set; }
}

public class GrammarCheckResponse
{
    public string OriginalText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string CorrectedText { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public IReadOnlyList<string> Suggestions { get; set; } = [];
    public IReadOnlyList<string> GrammarPoints { get; set; } = [];
}

public class WordExplanationRequest
{
    [Required(ErrorMessage = "Từ/cụm từ là bắt buộc")]
    [MaxLength(200)]
    public string Term { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã ngôn ngữ là bắt buộc (en, zh, ja, ko)")]
    public string LanguageCode { get; set; } = "en";

    public string? ContextSentence { get; set; }
}

public class AiSampleSentence
{
    public string Sentence { get; set; } = string.Empty;
    public string? Reading { get; set; }
    public string Translation { get; set; } = string.Empty;
}

public class WordExplanationResponse
{
    public string Term { get; set; } = string.Empty;
    public string? Reading { get; set; }
    public string Meaning { get; set; } = string.Empty;
    public string DetailedExplanation { get; set; } = string.Empty;
    public IReadOnlyList<AiSampleSentence> SampleSentences { get; set; } = [];
    public IReadOnlyList<string> CommonCollocations { get; set; } = [];
}

public class ChatMessageDto
{
    [Required]
    public string Role { get; set; } = "user"; // "user" | "assistant"

    [Required]
    public string Content { get; set; } = string.Empty;
}

public class AiChatRequest
{
    [Required(ErrorMessage = "Mã ngôn ngữ là bắt buộc")]
    public string LanguageCode { get; set; } = "en";

    public string Scenario { get; set; } = "casual_talk"; // casual_talk, restaurant, airport, shopping, job_interview
    public string? LevelCode { get; set; } = "A1";

    [Required(ErrorMessage = "Lịch sử tin nhắn là bắt buộc")]
    public IReadOnlyList<ChatMessageDto> Messages { get; set; } = [];
}

public class AiChatResponse
{
    public string Reply { get; set; } = string.Empty;
    public string? Reading { get; set; }
    public string VietnameseTranslation { get; set; } = string.Empty;
    public IReadOnlyList<string> SuggestedReplies { get; set; } = [];
}

public class GeneratedVocabItemDto
{
    public string Term { get; set; } = string.Empty;
    public string? Reading { get; set; }
    public string Meaning { get; set; } = string.Empty;
    public string PartOfSpeech { get; set; } = "noun";
    public string ExampleSentence { get; set; } = string.Empty;
    public string? ExampleReading { get; set; }
    public string ExampleTranslation { get; set; } = string.Empty;
}
