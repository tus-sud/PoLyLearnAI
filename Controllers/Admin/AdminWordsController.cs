using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Admin;
using PolyLearnAI.Api.Models.DTOs.Word;
using PolyLearnAI.Api.Models.Entities;
using PolyLearnAI.Api.Services.Implementations;

namespace PolyLearnAI.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/words")]
[Authorize(Roles = "Admin")]
public class AdminWordsController(AppDbContext dbContext) : ControllerBase
{
    /// <summary>
    /// Thêm từ vựng mới kèm câu ví dụ
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<WordDetailDto>> CreateWord(
        [FromBody] CreateWordRequest request,
        CancellationToken ct)
    {
        if (!VocabHarvesterService.IsValidVocabularyWord(request.Term, request.Meaning))
        {
            throw new BadHttpRequestException("Từ vựng hoặc nghĩa tiếng Việt không hợp lệ (không được chỉ chứa chữ số hoặc ký hiệu từ loại như n, v, adj, adv).");
        }

        var langExists = await dbContext.Languages.AnyAsync(l => l.Id == request.LanguageId, ct);
        if (!langExists) throw new BadHttpRequestException($"Ngôn ngữ ID {request.LanguageId} không tồn tại.");

        var levelExists = await dbContext.Levels.AnyAsync(lv => lv.Id == request.LevelId && lv.LanguageId == request.LanguageId, ct);
        if (!levelExists) throw new BadHttpRequestException($"Cấp độ ID {request.LevelId} không thuộc ngôn ngữ này.");

        if (request.TopicId.HasValue)
        {
            var topicExists = await dbContext.Topics.AnyAsync(t => t.Id == request.TopicId.Value && t.LanguageId == request.LanguageId, ct);
            if (!topicExists) throw new BadHttpRequestException($"Chủ đề ID {request.TopicId} không thuộc ngôn ngữ này.");
        }

        var word = new Word
        {
            LanguageId = request.LanguageId,
            LevelId = request.LevelId,
            TopicId = request.TopicId,
            Term = request.Term.Trim(),
            Reading = request.Reading?.Trim(),
            Meaning = request.Meaning.Trim(),
            PartOfSpeech = request.PartOfSpeech?.Trim(),
            CreatedAt = DateTime.UtcNow,
            Examples = request.Examples.Select(ex => new WordExample
            {
                Sentence = ex.Sentence.Trim(),
                Reading = ex.Reading?.Trim(),
                Translation = ex.Translation.Trim()
            }).ToList()
        };

        dbContext.Words.Add(word);
        await dbContext.SaveChangesAsync(ct);

        await dbContext.Entry(word).Reference(w => w.Language).LoadAsync(ct);
        await dbContext.Entry(word).Reference(w => w.Level).LoadAsync(ct);
        if (word.TopicId.HasValue) await dbContext.Entry(word).Reference(w => w.Topic).LoadAsync(ct);

        return CreatedAtAction(nameof(CreateWord), new { id = word.Id }, MapToDetailDto(word));
    }

    /// <summary>
    /// Nhập hàng loạt từ vựng (Batch / CSV import)
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult> BatchCreateWords(
        [FromBody] IReadOnlyList<CreateWordRequest> requests,
        CancellationToken ct)
    {
        if (requests.Count == 0) return BadRequest("Danh sách từ vựng trống.");

        var insertedCount = 0;
        var skippedCount = 0;
        foreach (var req in requests)
        {
            if (!VocabHarvesterService.IsValidVocabularyWord(req.Term, req.Meaning))
            {
                skippedCount++;
                continue;
            }

            var word = new Word
            {
                LanguageId = req.LanguageId,
                LevelId = req.LevelId,
                TopicId = req.TopicId,
                Term = req.Term.Trim(),
                Reading = req.Reading?.Trim(),
                Meaning = req.Meaning.Trim(),
                PartOfSpeech = req.PartOfSpeech?.Trim(),
                CreatedAt = DateTime.UtcNow,
                Examples = req.Examples.Select(ex => new WordExample
                {
                    Sentence = ex.Sentence.Trim(),
                    Reading = ex.Reading?.Trim(),
                    Translation = ex.Translation.Trim()
                }).ToList()
            };
            dbContext.Words.Add(word);
            insertedCount++;
        }

        if (insertedCount > 0)
        {
            await dbContext.SaveChangesAsync(ct);
        }

        var skipNote = skippedCount > 0 ? $" (Đã tự động bỏ qua {skippedCount} dòng lỗi cột số/thiếu nghĩa)" : "";
        return Ok(new { message = $"Đã nhập thành công {insertedCount} từ vựng vào hệ thống.{skipNote}", count = insertedCount, skipped = skippedCount });
    }

    /// <summary>
    /// Danh sách các bộ từ vựng chuẩn quốc tế có sẵn để nạp
    /// </summary>
    [HttpGet("presets")]
    public ActionResult GetAvailablePresets()
    {
        var list = StandardPresets.Datasets.Select(kv => new
        {
            Key = kv.Key,
            Name = kv.Value.Name,
            Description = kv.Value.Description,
            LanguageId = kv.Value.LanguageId,
            WordCount = kv.Value.Words.Count
        }).ToList();

        return Ok(list);
    }

    /// <summary>
    /// Nạp bộ từ vựng chuẩn quốc tế (Oxford, HSK, JLPT, TOPIK hoặc Tất cả)
    /// </summary>
    [HttpPost("presets/import")]
    public async Task<ActionResult> ImportPreset(
        [FromQuery] string datasetKey,
        CancellationToken ct)
    {
        var keysToImport = datasetKey.ToLowerInvariant() == "all"
            ? StandardPresets.Datasets.Keys.ToList()
            : [datasetKey.ToLowerInvariant()];

        var totalWordsImported = 0;
        var totalQuestionsCreated = 0;

        foreach (var key in keysToImport)
        {
            if (!StandardPresets.Datasets.TryGetValue(key, out var dataset)) continue;

            var langId = dataset.LanguageId;
            var levels = await dbContext.Levels
                .Where(l => l.LanguageId == langId)
                .OrderBy(l => l.SortOrder)
                .ToListAsync(ct);

            var topics = await dbContext.Topics
                .Where(t => t.LanguageId == langId)
                .OrderBy(t => t.Id)
                .ToListAsync(ct);

            var existingTerms = await dbContext.Words
                .Where(w => w.LanguageId == langId)
                .Select(w => w.Term.ToLower())
                .ToListAsync(ct);

            var wordsList = dataset.Words;
            for (int i = 0; i < wordsList.Count; i++)
            {
                var item = wordsList[i];
                if (existingTerms.Contains(item.Term.ToLower())) continue;

                var level = item.LevelOffset < levels.Count ? levels[item.LevelOffset] : levels[0];
                var topic = item.TopicOffset < topics.Count ? topics[item.TopicOffset] : topics[0];

                var word = new Word
                {
                    LanguageId = langId,
                    LevelId = level.Id,
                    TopicId = topic.Id,
                    Term = item.Term.Trim(),
                    Reading = item.Reading?.Trim(),
                    Meaning = item.Meaning.Trim(),
                    PartOfSpeech = item.PartOfSpeech.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    Examples =
                    [
                        new WordExample
                        {
                            Sentence = item.Example.Trim(),
                            Reading = item.ExampleReading?.Trim(),
                            Translation = item.ExampleVi.Trim()
                        }
                    ]
                };

                dbContext.Words.Add(word);
                existingTerms.Add(item.Term.ToLower());
                totalWordsImported++;

                var q = new Question
                {
                    LanguageId = langId,
                    LevelId = level.Id,
                    Text = $"Nghĩa của từ \"{item.Term}\" là gì?",
                    Explanation = $"\"{item.Term}\" nghĩa là \"{item.Meaning}\"."
                };

                q.Options.Add(new QuestionOption { Text = item.Meaning, IsCorrect = true });
                for (int d = 1; d <= 3; d++)
                {
                    var wrongMeaning = wordsList[(i + d) % wordsList.Count].Meaning;
                    if (wrongMeaning == item.Meaning) continue;
                    q.Options.Add(new QuestionOption { Text = wrongMeaning, IsCorrect = false });
                }

                dbContext.Questions.Add(q);
                totalQuestionsCreated++;
            }
        }

        await dbContext.SaveChangesAsync(ct);

        var message = totalWordsImported > 0
            ? $"Đã nạp thành công {totalWordsImported} từ vựng chuẩn quốc tế và tạo mới {totalQuestionsCreated} câu hỏi trắc nghiệm củng cố!"
            : "Tất cả từ vựng trong bộ này đã tồn tại trong hệ thống (đã được nạp trước đó), hệ thống tự động bỏ qua để tránh trùng lặp dữ liệu!";

        return Ok(new
        {
            message,
            wordsCount = totalWordsImported,
            questionsCount = totalQuestionsCreated
        });
    }

    /// <summary>
    /// Cập nhật từ vựng và danh sách câu ví dụ
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<WordDetailDto>> UpdateWord(
        int id,
        [FromBody] UpdateWordRequest request,
        CancellationToken ct)
    {
        var word = await dbContext.Words
            .Include(w => w.Language)
            .Include(w => w.Level)
            .Include(w => w.Topic)
            .Include(w => w.Examples)
            .FirstOrDefaultAsync(w => w.Id == id, ct);

        if (word == null) throw new KeyNotFoundException($"Không tìm thấy từ vựng ID {id}.");

        var levelExists = await dbContext.Levels.AnyAsync(lv => lv.Id == request.LevelId && lv.LanguageId == word.LanguageId, ct);
        if (!levelExists) throw new BadHttpRequestException($"Cấp độ ID {request.LevelId} không hợp lệ.");

        if (request.TopicId.HasValue)
        {
            var topicExists = await dbContext.Topics.AnyAsync(t => t.Id == request.TopicId.Value && t.LanguageId == word.LanguageId, ct);
            if (!topicExists) throw new BadHttpRequestException($"Chủ đề ID {request.TopicId} không hợp lệ.");
        }

        word.LevelId = request.LevelId;
        word.TopicId = request.TopicId;
        word.Term = request.Term.Trim();
        word.Reading = request.Reading?.Trim();
        word.Meaning = request.Meaning.Trim();
        word.PartOfSpeech = request.PartOfSpeech?.Trim();

        dbContext.WordExamples.RemoveRange(word.Examples);
        word.Examples = request.Examples.Select(ex => new WordExample
        {
            WordId = word.Id,
            Sentence = ex.Sentence.Trim(),
            Reading = ex.Reading?.Trim(),
            Translation = ex.Translation.Trim()
        }).ToList();

        await dbContext.SaveChangesAsync(ct);

        await dbContext.Entry(word).Reference(w => w.Level).LoadAsync(ct);
        if (word.TopicId.HasValue) await dbContext.Entry(word).Reference(w => w.Topic).LoadAsync(ct);

        return Ok(MapToDetailDto(word));
    }

    /// <summary>
    /// Xóa từ vựng
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWord(int id, CancellationToken ct)
    {
        var word = await dbContext.Words.FirstOrDefaultAsync(w => w.Id == id, ct);
        if (word == null) throw new KeyNotFoundException($"Không tìm thấy từ vựng ID {id}.");

        dbContext.Words.Remove(word);
        await dbContext.SaveChangesAsync(ct);

        return Ok(new { message = $"Đã xóa thành công từ vựng ID {id}." });
    }

    private static WordDetailDto MapToDetailDto(Word word) => new()
    {
        Id = word.Id,
        LanguageId = word.LanguageId,
        LanguageCode = word.Language.Code,
        LevelId = word.LevelId,
        LevelCode = word.Level.Code,
        LevelName = word.Level.Name,
        TopicId = word.TopicId,
        TopicName = word.Topic?.Name,
        Term = word.Term,
        Reading = word.Reading,
        Meaning = word.Meaning,
        PartOfSpeech = word.PartOfSpeech,
        ExamplesCount = word.Examples.Count,
        CreatedAt = word.CreatedAt,
        Examples = word.Examples.Select(ex => new WordExampleDto
        {
            Id = ex.Id,
            WordId = ex.WordId,
            Sentence = ex.Sentence,
            Reading = ex.Reading,
            Translation = ex.Translation
        }).ToList()
    };
}
