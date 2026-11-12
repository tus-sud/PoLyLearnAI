namespace PolyLearnAI.Api.Models.Entities;

public enum WordStatus { New = 0, Learning = 1, Mastered = 2 }

public class FavoriteWord
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int WordId { get; set; }
    public Word Word { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>Tiến độ ghi nhớ từng từ (flashcard, lặp lại ngắt quãng kiểu Leitner).</summary>
public class UserWordProgress
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int WordId { get; set; }
    public Word Word { get; set; } = null!;
    public WordStatus Status { get; set; } = WordStatus.New;
    /// <summary>Hộp Leitner 0..5</summary>
    public int Box { get; set; }
    public int ReviewCount { get; set; }
    public DateTime? LastReviewedAt { get; set; }
    public DateTime? NextReviewAt { get; set; }
}

public class LessonProgress
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class QuizAttempt
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int LanguageId { get; set; }
    public Language Language { get; set; } = null!;
    public int? LevelId { get; set; }
    public Level? Level { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public ICollection<QuizAnswer> Answers { get; set; } = [];
}

public class QuizAnswer
{
    public int Id { get; set; }
    public int QuizAttemptId { get; set; }
    public QuizAttempt QuizAttempt { get; set; } = null!;
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public int? SelectedOptionId { get; set; }
    public QuestionOption? SelectedOption { get; set; }
    public bool IsCorrect { get; set; }
}
