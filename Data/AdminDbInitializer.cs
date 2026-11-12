using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Models.Entities;
using PolyLearnAI.Api.Services.Implementations;
using PolyLearnAI.Api.Services.Interfaces;

namespace PolyLearnAI.Api.Data;

public static class AdminDbInitializer
{
    public static async Task SeedAdminUserAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AdminDbInitializer");

        var adminEmail = config["Admin:DefaultEmail"] ?? "admin@polylearn.ai";
        var adminPassword = config["Admin:DefaultPassword"] ?? "Admin@123456";

        try
        {
            logger.LogInformation("Đang kiểm tra và tự động áp dụng Database Migrations...");
            if (db.Database.IsSqlite())
            {
                await db.Database.EnsureCreatedAsync();
            }
            else
            {
                await db.Database.MigrateAsync();
            }
            logger.LogInformation("Database đã sẵn sàng và cập nhật mới nhất.");

            var adminExists = await db.Users.AnyAsync(u => u.Email == adminEmail);
            if (!adminExists)
            {
                var adminUser = new User
                {
                    Email = adminEmail.Trim().ToLowerInvariant(),
                    DisplayName = "Quản trị viên",
                    PasswordHash = hasher.HashPassword(adminPassword),
                    Role = UserRole.Admin,
                    IsActive = true,
                    PreferredLanguageId = 1,
                    CreatedAt = DateTime.UtcNow
                };

                db.Users.Add(adminUser);
                await db.SaveChangesAsync();
                logger.LogInformation("Đã khởi tạo tài khoản Admin mặc định: {Email}", adminEmail);
            }

            await PurgeCorruptedCsvDataAsync(db, logger);
            await EnsureFullLevelsAndBaseBankAsync(db, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Lỗi xảy ra khi seed tài khoản Admin");
        }
    }

    /// <summary>
    /// Đảm bảo đầy đủ các cấp độ chuẩn quốc tế (HSK 1–6, CEFR A1–C1, JLPT N5–N1, TOPIK 1–5)
    /// và kho từ vựng nền tảng phong phú để thi thử đề 45–100 câu (Nghe • Đọc • Viết).
    /// </summary>
    private static async Task EnsureFullLevelsAndBaseBankAsync(AppDbContext db, ILogger logger)
    {
        var fullLevelsSpec = new (int LangId, string Code, string Name, int SortOrder)[]
        {
            (1, "A1", "CEFR A1", 1), (1, "A2", "CEFR A2", 2), (1, "B1", "CEFR B1", 3), (1, "B2", "CEFR B2", 4), (1, "C1", "CEFR C1", 5),
            (2, "HSK1", "HSK 1", 1), (2, "HSK2", "HSK 2", 2), (2, "HSK3", "HSK 3", 3), (2, "HSK4", "HSK 4", 4), (2, "HSK5", "HSK 5", 5), (2, "HSK6", "HSK 6", 6),
            (3, "N5", "JLPT N5", 1), (3, "N4", "JLPT N4", 2), (3, "N3", "JLPT N3", 3), (3, "N2", "JLPT N2", 4), (3, "N1", "JLPT N1", 5),
            (4, "TOPIK1", "TOPIK 1", 1), (4, "TOPIK2", "TOPIK 2", 2), (4, "TOPIK3", "TOPIK 3", 3), (4, "TOPIK4", "TOPIK 4", 4), (4, "TOPIK5", "TOPIK 5", 5)
        };

        var existingLevels = await db.Levels.ToListAsync();
        var addedLevels = 0;
        foreach (var spec in fullLevelsSpec)
        {
            var exists = existingLevels.Any(l =>
                l.LanguageId == spec.LangId &&
                l.Code.Equals(spec.Code, StringComparison.OrdinalIgnoreCase));
            if (!exists)
            {
                db.Levels.Add(new Level
                {
                    LanguageId = spec.LangId,
                    Code = spec.Code,
                    Name = spec.Name,
                    SortOrder = spec.SortOrder
                });
                addedLevels++;
            }
        }

        if (addedLevels > 0)
        {
            await db.SaveChangesAsync();
            logger.LogInformation("Đã bổ sung thêm {Count} cấp độ thi chuẩn quốc tế (HSK/CEFR/JLPT/TOPIK).", addedLevels);
        }

        var allLevelsByLang = (await db.Levels.OrderBy(l => l.SortOrder).ToListAsync())
            .GroupBy(l => l.LanguageId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var allTopicsByLang = (await db.Topics.OrderBy(t => t.Id).ToListAsync())
            .GroupBy(t => t.LanguageId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var addedWords = 0;
        foreach (var langId in new[] { 1, 2, 3, 4 })
        {
            if (!allLevelsByLang.TryGetValue(langId, out var langLevels) || langLevels.Count == 0) continue;
            var langTopics = allTopicsByLang.GetValueOrDefault(langId) ?? [];

            var existingTerms = (await db.Words
                .Where(w => w.LanguageId == langId)
                .Select(w => w.Term)
                .ToListAsync())
                .Select(t => t.Trim().ToLowerInvariant())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var presetKey = langId switch { 1 => "oxford", 2 => "hsk", 3 => "jlpt", 4 => "topik", _ => "" };
            if (StandardPresets.Datasets.TryGetValue(presetKey, out var presetDs))
            {
                for (var i = 0; i < presetDs.Words.Count; i++)
                {
                    var p = presetDs.Words[i];
                    var key = p.Term.Trim().ToLowerInvariant();
                    if (!existingTerms.Add(key)) continue;

                    var lvl = langLevels[i % langLevels.Count];
                    var top = langTopics.Count > 0 ? langTopics[p.TopicOffset % langTopics.Count] : null;
                    db.Words.Add(new Word
                    {
                        LanguageId = langId,
                        LevelId = lvl.Id,
                        TopicId = top?.Id,
                        Term = p.Term.Trim(),
                        Reading = p.Reading?.Trim(),
                        Meaning = p.Meaning.Trim(),
                        PartOfSpeech = p.PartOfSpeech.Trim(),
                        CreatedAt = DateTime.UtcNow,
                        Examples =
                        [
                            new WordExample
                            {
                                Sentence = p.Example.Trim(),
                                Reading = p.ExampleReading?.Trim(),
                                Translation = p.ExampleVi.Trim()
                            }
                        ]
                    });
                    addedWords++;
                }
            }

            var vaultWords = EndlessVocabBank.Words.Where(v => v.LanguageId == langId).ToList();
            for (var i = 0; i < vaultWords.Count; i++)
            {
                var v = vaultWords[i];
                var key = v.Term.Trim().ToLowerInvariant();
                if (!existingTerms.Add(key)) continue;

                var lvl = langLevels[(i + 1) % langLevels.Count];
                var top = langTopics.Count > 0 ? langTopics[v.TopicOffset % langTopics.Count] : null;
                db.Words.Add(new Word
                {
                    LanguageId = langId,
                    LevelId = lvl.Id,
                    TopicId = top?.Id,
                    Term = v.Term.Trim(),
                    Reading = v.Reading?.Trim(),
                    Meaning = v.Meaning.Trim(),
                    PartOfSpeech = v.PartOfSpeech.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    Examples =
                    [
                        new WordExample
                        {
                            Sentence = v.Example.Trim(),
                            Reading = v.ExampleReading?.Trim(),
                            Translation = v.ExampleVi.Trim()
                        }
                    ]
                });
                addedWords++;
            }
        }

        if (addedWords > 0)
        {
            await db.SaveChangesAsync();
            logger.LogInformation("Đã nạp sẵn {Count} từ vựng chuẩn quốc tế vào kho đề thi 4 ngôn ngữ.", addedWords);
        }
    }

    /// <summary>
    /// Tự động dọn sạch các bản ghi từ vựng &amp; câu hỏi bị lỗi lệch cột khi nhập CSV trước đây
    /// (VD: Term là số thứ tự "32", "36", "88", "96" hoặc Meaning là ký hiệu từ loại "v", "adv", "n").
    /// </summary>
    private static async Task PurgeCorruptedCsvDataAsync(AppDbContext db, ILogger logger)
    {
        var allWords = await db.Words.ToListAsync();
        var invalidWords = allWords
            .Where(w => !VocabHarvesterService.IsValidVocabularyWord(w.Term, w.Meaning))
            .ToList();

        if (invalidWords.Count > 0)
        {
            var badWordIds = invalidWords.Select(w => w.Id).ToHashSet();

            var favs = await db.FavoriteWords.Where(f => badWordIds.Contains(f.WordId)).ToListAsync();
            if (favs.Count > 0) db.FavoriteWords.RemoveRange(favs);

            var progresses = await db.UserWordProgress.Where(p => badWordIds.Contains(p.WordId)).ToListAsync();
            if (progresses.Count > 0) db.UserWordProgress.RemoveRange(progresses);

            db.Words.RemoveRange(invalidWords);
            await db.SaveChangesAsync();
            logger.LogInformation("Đã dọn sạch {Count} từ vựng bị lỗi định dạng số/cột CSV khỏi CSDL.", invalidWords.Count);
        }

        var candidateQuestions = await db.Questions
            .Include(q => q.Options)
            .Where(q => !q.Text.StartsWith("[TL:"))
            .ToListAsync();

        var dirtyQuestions = candidateQuestions
            .Where(q => !VocabHarvesterService.IsCleanMcqQuestion(q))
            .ToList();

        if (dirtyQuestions.Count > 0)
        {
            var dirtyIds = dirtyQuestions.Select(q => q.Id).ToHashSet();
            var referencedIds = (await db.QuizAnswers
                .Where(a => dirtyIds.Contains(a.QuestionId))
                .Select(a => a.QuestionId)
                .Distinct()
                .ToListAsync())
                .ToHashSet();

            var deletableQuestions = dirtyQuestions.Where(q => !referencedIds.Contains(q.Id)).ToList();
            if (deletableQuestions.Count > 0)
            {
                db.Questions.RemoveRange(deletableQuestions);
                await db.SaveChangesAsync();
                logger.LogInformation("Đã dọn sạch {Count} câu hỏi trắc nghiệm nhiễu số khỏi CSDL.", deletableQuestions.Count);
            }
        }
    }
}
