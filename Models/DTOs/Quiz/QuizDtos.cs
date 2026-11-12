using System.ComponentModel.DataAnnotations;
using PolyLearnAI.Api.Models.DTOs.Common;
using PolyLearnAI.Api.Models.DTOs.Word;

namespace PolyLearnAI.Api.Models.DTOs.Quiz;

public class GenerateQuizRequest
{
    [Required]
    public int LanguageId { get; set; }
    public int? LevelId { get; set; }
    public int? LessonId { get; set; }
    public int QuestionCount { get; set; } = 10;

    /// <summary>
    /// "vocab_random": tạo đề ngẫu nhiên đa chiều trực tiếp từ kho từ vựng,
    /// "mixed": kết hợp ngân hàng đề + từ vựng ngẫu nhiên,
    /// "bank": chỉ lấy từ ngân hàng đề hiện có.
    /// </summary>
    public string Mode { get; set; } = "vocab_random";

    /// <summary>
    /// Hình thức kiểm tra:
    /// "official_mock": Đề thi chuẩn quốc tế 3 kỹ năng (🎧 Nghe + 📖 Đọc + ✏️ Viết) có bấm giờ
    /// "combined" (mặc định): Kết hợp Trắc nghiệm (A/B/C/D) + Tự luận (Viết từ, Điền câu, Đặt câu)
    /// "mcq": Chỉ Trắc nghiệm
    /// "written": Chỉ Tự luận
    /// </summary>
    public string ExamFormat { get; set; } = "combined";

    /// <summary>
    /// Số thứ tự đề thi chuẩn (VD: Đề 1 .. Đề 10) để xoay vòng tập từ vựng tương ứng.
    /// </summary>
    public int? ExamNumber { get; set; }

    public int? DurationMinutes { get; set; }
    public int? ListeningCount { get; set; }
    public int? ReadingCount { get; set; }
    public int? WritingCount { get; set; }

    /// <summary>
    /// Tự động săn thêm từ mới (AI / Từ điển ngoài / Kho mở rộng) và lưu vĩnh viễn vào CSDL trước khi tạo đề.
    /// </summary>
    public bool HarvestNewWords { get; set; } = false;

    public int NewWordsCount { get; set; } = 5;

    [MaxLength(120)]
    public string? TopicHint { get; set; }
}

public class QuizOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

public class QuizQuestionDto
{
    public int QuestionId { get; set; }
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// "mcq" (Trắc nghiệm chọn A/B/C/D) | "written" (Tự luận tự nhập đáp án / đặt câu)
    /// </summary>
    public string QuestionType { get; set; } = "mcq";

    /// <summary>
    /// Kỹ năng của câu hỏi: "listening" (🎧 Nghe) | "reading" (📖 Đọc) | "writing" (✏️ Viết)
    /// </summary>
    public string Section { get; set; } = "reading";

    /// <summary>
    /// Nội dung âm thanh phát qua giọng đọc chuẩn cho phần thi Nghe (🎧 Listening)
    /// </summary>
    public string? AudioText { get; set; }

    /// <summary>
    /// "write_term" | "cloze" | "make_sentence" (dành cho câu tự luận)
    /// </summary>
    public string? WrittenSubtype { get; set; }

    /// <summary>
    /// Gợi ý độ dài từ, chữ cái đầu hoặc yêu cầu đặt câu
    /// </summary>
    public string? Hint { get; set; }

    /// <summary>
    /// Từ mục tiêu cho trước (chỉ điền đối với dạng tự luận đặt câu "make_sentence")
    /// </summary>
    public string? TargetTerm { get; set; }

    public IReadOnlyList<QuizOptionDto> Options { get; set; } = [];
}

public class QuizSessionDto
{
    public int AttemptId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public int? LevelId { get; set; }
    public string? LevelCode { get; set; }
    public string ExamFormat { get; set; } = "combined";
    public int? ExamNumber { get; set; }
    public int? DurationMinutes { get; set; }
    public int TotalQuestions { get; set; }
    public int ListeningCount { get; set; }
    public int ReadingCount { get; set; }
    public int WritingCount { get; set; }
    public int McqCount { get; set; }
    public int WrittenCount { get; set; }
    public DateTime StartedAt { get; set; }
    public int NewWordsHarvestedCount { get; set; }
    public int TotalWordsInBank { get; set; }
    public string? HarvestSource { get; set; }
    public IReadOnlyList<WordDto> NewlyHarvestedWords { get; set; } = [];
    public IReadOnlyList<QuizQuestionDto> Questions { get; set; } = [];
}

public class SubmitAnswerDto
{
    [Required]
    public int QuestionId { get; set; }
    public int? SelectedOptionId { get; set; }

    [MaxLength(1000)]
    public string? WrittenAnswer { get; set; }
}

public class SubmitQuizRequest
{
    [Required]
    public IReadOnlyList<SubmitAnswerDto> Answers { get; set; } = [];
}

public class QuizAnswerDetailDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string QuestionType { get; set; } = "mcq";
    public string Section { get; set; } = "reading";
    public int? SelectedOptionId { get; set; }
    public string? SelectedOptionText { get; set; }
    public string? UserWrittenAnswer { get; set; }
    public int CorrectOptionId { get; set; }
    public string CorrectOptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public string? Explanation { get; set; }
    public string? AiFeedback { get; set; }
}

public class QuizResultDto
{
    public int AttemptId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public int? LevelId { get; set; }
    public string? LevelCode { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public int ListeningCorrect { get; set; }
    public int ListeningTotal { get; set; }
    public int ReadingCorrect { get; set; }
    public int ReadingTotal { get; set; }
    public int McqCorrect { get; set; }
    public int McqTotal { get; set; }
    public int WrittenCorrect { get; set; }
    public int WrittenTotal { get; set; }
    public double Percentage => TotalQuestions > 0 ? Math.Round((double)Score / TotalQuestions * 100, 1) : 0;
    public bool IsPassed => Percentage >= 70;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int XpEarned { get; set; }
    public int TotalXp { get; set; }
    public int TodayXp { get; set; }
    public int DailyGoalXp { get; set; }
    public int Level { get; set; }
    public bool LeveledUp { get; set; }
    public IReadOnlyList<QuizAnswerDetailDto> Details { get; set; } = [];
}

public class QuizHistoryDto
{
    public int AttemptId { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public int? LevelId { get; set; }
    public string? LevelCode { get; set; }
    public int Score { get; set; }
    public int TotalQuestions { get; set; }
    public double Percentage => TotalQuestions > 0 ? Math.Round((double)Score / TotalQuestions * 100, 1) : 0;
    public bool IsPassed => Percentage >= 70;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
