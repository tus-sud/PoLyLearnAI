using PolyLearnAI.Api.Models.DTOs.Common;
using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Models.DTOs.Lesson;

public class LessonFilterParams : PaginationParams
{
    public int? LanguageId { get; set; }
    public string? LanguageCode { get; set; }
    public int? LevelId { get; set; }
    public LessonType? Type { get; set; }
    public string? Search { get; set; }
}

public class LessonDto
{
    public int Id { get; set; }
    public int LanguageId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
    public int LevelId { get; set; }
    public string LevelCode { get; set; } = string.Empty;
    public string LevelName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public LessonType Type { get; set; }
    public string TypeName => Type.ToString();
    public string? ContentPreview { get; set; }
    public int SortOrder { get; set; }
    public int QuestionCount { get; set; }
    public bool? IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LessonQuestionOptionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}

public class LessonQuestionDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public IReadOnlyList<LessonQuestionOptionDto> Options { get; set; } = [];
}

public class LessonDetailDto : LessonDto
{
    public string? Content { get; set; }
    public IReadOnlyList<LessonQuestionDto> Questions { get; set; } = [];
}
