namespace PolyLearnAI.Api.Models.Entities;

public enum LessonType { Vocabulary = 0, Listening = 1, Reading = 2 }

public class Lesson
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
    public int LevelId { get; set; }
    public Level Level { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public LessonType Type { get; set; }
    /// <summary>Đoạn văn đọc hiểu, hoặc văn bản để TTS đọc cho bài nghe.</summary>
    public string? Content { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Question> Questions { get; set; } = [];
}

public class Question
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
    public int LevelId { get; set; }
    public Level Level { get; set; } = null!;
    /// <summary>Null = câu hỏi thuộc ngân hàng đề chung, không gắn bài học.</summary>
    public int? LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }

    public ICollection<QuestionOption> Options { get; set; } = [];
}

public class QuestionOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
