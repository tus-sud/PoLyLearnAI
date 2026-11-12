using System.ComponentModel.DataAnnotations;
using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Models.DTOs.Admin;

public class CreateWordExampleDto
{
    [Required]
    [MaxLength(1000)]
    public string Sentence { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Reading { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Translation { get; set; } = string.Empty;
}

public class CreateWordRequest
{
    [Required]
    public int LanguageId { get; set; }

    [Required]
    public int LevelId { get; set; }

    public int? TopicId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Term { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Reading { get; set; }

    [Required]
    [MaxLength(500)]
    public string Meaning { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? PartOfSpeech { get; set; }

    public IReadOnlyList<CreateWordExampleDto> Examples { get; set; } = [];
}

public class UpdateWordRequest
{
    [Required]
    public int LevelId { get; set; }

    public int? TopicId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Term { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Reading { get; set; }

    [Required]
    [MaxLength(500)]
    public string Meaning { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? PartOfSpeech { get; set; }

    public IReadOnlyList<CreateWordExampleDto> Examples { get; set; } = [];
}

public class CreateLessonRequest
{
    [Required]
    public int LanguageId { get; set; }

    [Required]
    public int LevelId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public LessonType Type { get; set; }

    public string? Content { get; set; }

    public int SortOrder { get; set; } = 1;

    public bool IsPublished { get; set; } = true;
}

public class UpdateLessonRequest
{
    [Required]
    public int LevelId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public LessonType Type { get; set; }

    public string? Content { get; set; }

    public int SortOrder { get; set; }

    public bool IsPublished { get; set; }
}

public class CreateQuestionOptionDto
{
    [Required]
    [MaxLength(500)]
    public string Text { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}

public class CreateQuestionRequest
{
    [Required]
    public int LanguageId { get; set; }

    [Required]
    public int LevelId { get; set; }

    public int? LessonId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Explanation { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Cần tối thiểu 2 đáp án")]
    public IReadOnlyList<CreateQuestionOptionDto> Options { get; set; } = [];
}

public class UpdateQuestionRequest
{
    [Required]
    public int LevelId { get; set; }

    public int? LessonId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Explanation { get; set; }

    [Required]
    [MinLength(2, ErrorMessage = "Cần tối thiểu 2 đáp án")]
    public IReadOnlyList<CreateQuestionOptionDto> Options { get; set; } = [];
}

public class AdminUserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? PreferredLanguageCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class UpdateUserStatusRequest
{
    [Required]
    public bool IsActive { get; set; }
}

public class UpdateUserRoleRequest
{
    [Required]
    public UserRole Role { get; set; }
}
