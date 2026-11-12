using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Admin;
using PolyLearnAI.Api.Models.DTOs.Common;
using PolyLearnAI.Api.Models.DTOs.Lesson;
using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/questions")]
[Authorize(Roles = "Admin")]
public class AdminQuestionsController(AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách câu hỏi trắc nghiệm có phân trang và bộ lọc
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<LessonQuestionDto>>> GetQuestions(
        [FromQuery] PaginationParams pagination,
        [FromQuery] int? languageId,
        [FromQuery] int? levelId,
        [FromQuery] string? search,
        CancellationToken ct)
    {
        var query = dbContext.Questions
            .AsNoTracking()
            .Include(q => q.Options)
            .AsQueryable();

        if (languageId.HasValue)
        {
            query = query.Where(q => q.LanguageId == languageId.Value);
        }

        if (levelId.HasValue)
        {
            query = query.Where(q => q.LevelId == levelId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(q => q.Text.Contains(term) || (q.Explanation != null && q.Explanation.Contains(term)));
        }

        var totalCount = await query.CountAsync(ct);

        var questions = await query
            .OrderByDescending(q => q.Id)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync(ct);

        var items = questions.Select(MapToDto).ToList();

        return Ok(new PagedResult<LessonQuestionDto>(items, totalCount, pagination.PageNumber, pagination.PageSize));
    }

    /// <summary>
    /// Tạo câu hỏi trắc nghiệm mới (ngân hàng đề hoặc bài học)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LessonQuestionDto>> CreateQuestion(
        [FromBody] CreateQuestionRequest request,
        CancellationToken ct)
    {
        var langExists = await dbContext.Languages.AnyAsync(l => l.Id == request.LanguageId, ct);
        if (!langExists) throw new BadHttpRequestException($"Ngôn ngữ ID {request.LanguageId} không tồn tại.");

        var levelExists = await dbContext.Levels.AnyAsync(lv => lv.Id == request.LevelId && lv.LanguageId == request.LanguageId, ct);
        if (!levelExists) throw new BadHttpRequestException($"Cấp độ ID {request.LevelId} không thuộc ngôn ngữ này.");

        if (request.LessonId.HasValue)
        {
            var lessonExists = await dbContext.Lessons.AnyAsync(l => l.Id == request.LessonId.Value && l.LanguageId == request.LanguageId, ct);
            if (!lessonExists) throw new BadHttpRequestException($"Bài học ID {request.LessonId} không hợp lệ.");
        }

        if (!request.Options.Any(o => o.IsCorrect))
        {
            throw new BadHttpRequestException("Cần có ít nhất một đáp án đúng (IsCorrect = true).");
        }

        var question = new Question
        {
            LanguageId = request.LanguageId,
            LevelId = request.LevelId,
            LessonId = request.LessonId,
            Text = request.Text.Trim(),
            Explanation = request.Explanation?.Trim(),
            Options = request.Options.Select(o => new QuestionOption
            {
                Text = o.Text.Trim(),
                IsCorrect = o.IsCorrect
            }).ToList()
        };

        dbContext.Questions.Add(question);
        await dbContext.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(CreateQuestion), new { id = question.Id }, MapToDto(question));
    }

    /// <summary>
    /// Cập nhật câu hỏi và danh sách đáp án
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<LessonQuestionDto>> UpdateQuestion(
        int id,
        [FromBody] UpdateQuestionRequest request,
        CancellationToken ct)
    {
        var question = await dbContext.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == id, ct);

        if (question == null) throw new KeyNotFoundException($"Không tìm thấy câu hỏi ID {id}.");

        var levelExists = await dbContext.Levels.AnyAsync(lv => lv.Id == request.LevelId && lv.LanguageId == question.LanguageId, ct);
        if (!levelExists) throw new BadHttpRequestException($"Cấp độ ID {request.LevelId} không hợp lệ.");

        if (request.LessonId.HasValue)
        {
            var lessonExists = await dbContext.Lessons.AnyAsync(l => l.Id == request.LessonId.Value && l.LanguageId == question.LanguageId, ct);
            if (!lessonExists) throw new BadHttpRequestException($"Bài học ID {request.LessonId} không hợp lệ.");
        }

        if (!request.Options.Any(o => o.IsCorrect))
        {
            throw new BadHttpRequestException("Cần có ít nhất một đáp án đúng (IsCorrect = true).");
        }

        question.LevelId = request.LevelId;
        question.LessonId = request.LessonId;
        question.Text = request.Text.Trim();
        question.Explanation = request.Explanation?.Trim();

        dbContext.QuestionOptions.RemoveRange(question.Options);
        question.Options = request.Options.Select(o => new QuestionOption
        {
            QuestionId = question.Id,
            Text = o.Text.Trim(),
            IsCorrect = o.IsCorrect
        }).ToList();

        await dbContext.SaveChangesAsync(ct);

        return Ok(MapToDto(question));
    }

    /// <summary>
    /// Xóa câu hỏi
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteQuestion(int id, CancellationToken ct)
    {
        var question = await dbContext.Questions.FirstOrDefaultAsync(q => q.Id == id, ct);
        if (question == null) throw new KeyNotFoundException($"Không tìm thấy câu hỏi ID {id}.");

        dbContext.Questions.Remove(question);
        await dbContext.SaveChangesAsync(ct);

        return Ok(new { message = $"Đã xóa câu hỏi ID {id} thành công." });
    }

    private static LessonQuestionDto MapToDto(Question q) => new()
    {
        Id = q.Id,
        Text = q.Text,
        Explanation = q.Explanation,
        Options = q.Options.Select(o => new LessonQuestionOptionDto
        {
            Id = o.Id,
            Text = o.Text,
            IsCorrect = o.IsCorrect
        }).ToList()
    };
}
