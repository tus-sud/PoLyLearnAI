using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Language;

namespace PolyLearnAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LanguagesController(AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Lấy danh sách tất cả các ngôn ngữ hỗ trợ
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LanguageDto>>> GetAll(CancellationToken ct)
    {
        var list = await dbContext.Languages
            .AsNoTracking()
            .Select(l => new LanguageDto
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name,
                NativeName = l.NativeName,
                ProficiencySystem = l.ProficiencySystem,
                SpeechLocale = l.SpeechLocale,
                LevelCount = l.Levels.Count,
                TopicCount = l.Topics.Count
            })
            .ToListAsync(ct);

        return Ok(list);
    }

    /// <summary>
    /// Lấy chi tiết ngôn ngữ theo ID (kèm danh sách Levels và Topics)
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LanguageDetailDto>> GetById(int id, CancellationToken ct)
    {
        var lang = await dbContext.Languages
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LanguageDetailDto
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name,
                NativeName = l.NativeName,
                ProficiencySystem = l.ProficiencySystem,
                SpeechLocale = l.SpeechLocale,
                LevelCount = l.Levels.Count,
                TopicCount = l.Topics.Count,
                Levels = l.Levels.OrderBy(lv => lv.SortOrder).Select(lv => new LevelDto
                {
                    Id = lv.Id,
                    LanguageId = lv.LanguageId,
                    Code = lv.Code,
                    Name = lv.Name,
                    SortOrder = lv.SortOrder
                }).ToList(),
                Topics = l.Topics.Select(tp => new TopicDto
                {
                    Id = tp.Id,
                    LanguageId = tp.LanguageId,
                    Name = tp.Name,
                    Description = tp.Description,
                    Icon = tp.Icon,
                    WordCount = dbContext.Words.Count(w => w.TopicId == tp.Id)
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (lang == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy ngôn ngữ có ID {id}.");
        }

        return Ok(lang);
    }

    /// <summary>
    /// Lấy chi tiết ngôn ngữ theo Code (vd: en, zh, ja, ko)
    /// </summary>
    [HttpGet("code/{code}")]
    public async Task<ActionResult<LanguageDetailDto>> GetByCode(string code, CancellationToken ct)
    {
        var normalizedCode = code.Trim().ToLowerInvariant();
        var lang = await dbContext.Languages
            .AsNoTracking()
            .Where(l => l.Code == normalizedCode)
            .Select(l => new LanguageDetailDto
            {
                Id = l.Id,
                Code = l.Code,
                Name = l.Name,
                NativeName = l.NativeName,
                ProficiencySystem = l.ProficiencySystem,
                SpeechLocale = l.SpeechLocale,
                LevelCount = l.Levels.Count,
                TopicCount = l.Topics.Count,
                Levels = l.Levels.OrderBy(lv => lv.SortOrder).Select(lv => new LevelDto
                {
                    Id = lv.Id,
                    LanguageId = lv.LanguageId,
                    Code = lv.Code,
                    Name = lv.Name,
                    SortOrder = lv.SortOrder
                }).ToList(),
                Topics = l.Topics.Select(tp => new TopicDto
                {
                    Id = tp.Id,
                    LanguageId = tp.LanguageId,
                    Name = tp.Name,
                    Description = tp.Description,
                    Icon = tp.Icon,
                    WordCount = dbContext.Words.Count(w => w.TopicId == tp.Id)
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (lang == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy ngôn ngữ với mã '{code}'.");
        }

        return Ok(lang);
    }

    /// <summary>
    /// Lấy danh sách các cấp độ (Levels) của ngôn ngữ
    /// </summary>
    [HttpGet("{id:int}/levels")]
    public async Task<ActionResult<IReadOnlyList<LevelDto>>> GetLevels(int id, CancellationToken ct)
    {
        var levels = await dbContext.Levels
            .AsNoTracking()
            .Where(lv => lv.LanguageId == id)
            .OrderBy(lv => lv.SortOrder)
            .Select(lv => new LevelDto
            {
                Id = lv.Id,
                LanguageId = lv.LanguageId,
                Code = lv.Code,
                Name = lv.Name,
                SortOrder = lv.SortOrder
            })
            .ToListAsync(ct);

        return Ok(levels);
    }

    /// <summary>
    /// Lấy danh sách các chủ đề (Topics) của ngôn ngữ
    /// </summary>
    [HttpGet("{id:int}/topics")]
    public async Task<ActionResult<IReadOnlyList<TopicDto>>> GetTopics(int id, CancellationToken ct)
    {
        var topics = await dbContext.Topics
            .AsNoTracking()
            .Where(tp => tp.LanguageId == id)
            .Select(tp => new TopicDto
            {
                Id = tp.Id,
                LanguageId = tp.LanguageId,
                Name = tp.Name,
                Description = tp.Description,
                Icon = tp.Icon,
                WordCount = dbContext.Words.Count(w => w.TopicId == tp.Id)
            })
            .ToListAsync(ct);

        return Ok(topics);
    }
}
