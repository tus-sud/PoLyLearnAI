using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Admin;
using PolyLearnAI.Api.Models.DTOs.Lesson;
using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/lessons")]
[Authorize(Roles = "Admin")]
public class AdminLessonsController(AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Tạo bài học mới (Từ vựng, Luyện nghe, Đọc hiểu)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LessonDto>> CreateLesson(
        [FromBody] CreateLessonRequest request,
        CancellationToken ct)
    {
        var langExists = await dbContext.Languages.AnyAsync(l => l.Id == request.LanguageId, ct);
        if (!langExists) throw new BadHttpRequestException($"Ngôn ngữ ID {request.LanguageId} không tồn tại.");

        var levelExists = await dbContext.Levels.AnyAsync(lv => lv.Id == request.LevelId && lv.LanguageId == request.LanguageId, ct);
        if (!levelExists) throw new BadHttpRequestException($"Cấp độ ID {request.LevelId} không thuộc ngôn ngữ này.");

        var lesson = new Lesson
        {
            LanguageId = request.LanguageId,
            LevelId = request.LevelId,
            Title = request.Title.Trim(),
            Type = request.Type,
            Content = request.Content?.Trim(),
            SortOrder = request.SortOrder,
            IsPublished = request.IsPublished,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Lessons.Add(lesson);
        await dbContext.SaveChangesAsync(ct);

        await dbContext.Entry(lesson).Reference(l => l.Language).LoadAsync(ct);
        await dbContext.Entry(lesson).Reference(l => l.Level).LoadAsync(ct);

        return CreatedAtAction(nameof(CreateLesson), new { id = lesson.Id }, MapToDto(lesson));
    }

    /// <summary>
    /// Cập nhật thông tin bài học
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<LessonDto>> UpdateLesson(
        int id,
        [FromBody] UpdateLessonRequest request,
        CancellationToken ct)
    {
        var lesson = await dbContext.Lessons
            .Include(l => l.Language)
            .Include(l => l.Level)
            .Include(l => l.Questions)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (lesson == null) throw new KeyNotFoundException($"Không tìm thấy bài học ID {id}.");

        var levelExists = await dbContext.Levels.AnyAsync(lv => lv.Id == request.LevelId && lv.LanguageId == lesson.LanguageId, ct);
        if (!levelExists) throw new BadHttpRequestException($"Cấp độ ID {request.LevelId} không hợp lệ.");

        lesson.LevelId = request.LevelId;
        lesson.Title = request.Title.Trim();
        lesson.Type = request.Type;
        lesson.Content = request.Content?.Trim();
        lesson.SortOrder = request.SortOrder;
        lesson.IsPublished = request.IsPublished;

        await dbContext.SaveChangesAsync(ct);
        await dbContext.Entry(lesson).Reference(l => l.Level).LoadAsync(ct);

        return Ok(MapToDto(lesson));
    }

    /// <summary>
    /// Xóa bài học
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteLesson(int id, CancellationToken ct)
    {
        var lesson = await dbContext.Lessons.FirstOrDefaultAsync(l => l.Id == id, ct);
        if (lesson == null) throw new KeyNotFoundException($"Không tìm thấy bài học ID {id}.");

        dbContext.Lessons.Remove(lesson);
        await dbContext.SaveChangesAsync(ct);

        return Ok(new { message = $"Đã xóa bài học ID {id} thành công." });
    }

    private static LessonDto MapToDto(Lesson l) => new()
    {
        Id = l.Id,
        LanguageId = l.LanguageId,
        LanguageCode = l.Language.Code,
        LevelId = l.LevelId,
        LevelCode = l.Level.Code,
        LevelName = l.Level.Name,
        Title = l.Title,
        Type = l.Type,
        ContentPreview = l.Content != null && l.Content.Length > 120 ? l.Content[..120] + "..." : l.Content,
        SortOrder = l.SortOrder,
        QuestionCount = l.Questions.Count,
        CreatedAt = l.CreatedAt
    };
}
