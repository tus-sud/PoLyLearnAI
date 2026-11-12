using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Ai;
using PolyLearnAI.Api.Models.DTOs.Word;
using PolyLearnAI.Api.Models.Entities;
using PolyLearnAI.Api.Services.Interfaces;

namespace PolyLearnAI.Api.Services.Implementations;

public class VocabHarvesterService(
    AppDbContext dbContext,
    IAiService aiService,
    HttpClient httpClient,
    ILogger<VocabHarvesterService> logger) : IVocabHarvesterService
{
    private static readonly string[] RandomTopicSeeds =
    [
        "technology", "travel", "business", "food", "health", "culture",
        "science", "education", "nature", "music", "cinema", "sports", "finance"
    ];

    public static readonly HashSet<string> InvalidRawMeanings = new(StringComparer.OrdinalIgnoreCase)
    {
        "n", "v", "adj", "adv", "prep", "pron", "conj", "det", "interj", "num", "aux", "art",
        "noun", "verb", "adjective", "adverb", "preposition", "pronoun", "conjunction",
        "a1", "a2", "b1", "b2", "c1", "c2",
        "hsk1", "hsk2", "hsk3", "hsk4", "hsk5", "hsk6",
        "n5", "n4", "n3", "n2", "n1", "topik1", "topik2"
    };

    public static bool IsValidVocabularyWord(string? term, string? meaning)
    {
        if (string.IsNullOrWhiteSpace(term) || string.IsNullOrWhiteSpace(meaning))
        {
            return false;
        }

        var cleanTerm = term.Trim();
        var cleanMeaning = meaning.Trim();

        // Loại bỏ các dòng lỗi import CSV nơi Term hoặc Meaning chỉ toàn chữ số/ký tự đặc biệt hoặc bắt đầu bằng số thứ tự (VD: "36", "32", "96", "88")
        if (Regex.IsMatch(cleanTerm, @"^[\d\W_]+$") ||
            Regex.IsMatch(cleanMeaning, @"^[\d\W_]+$") ||
            Regex.IsMatch(cleanTerm, @"^\d+"))
        {
            return false;
        }

        // Loại bỏ trường hợp cột Meaning bị lệch sang nhãn từ loại hoặc mã cấp độ (VD: "v", "adv", "n", "adj", "A1")
        if (InvalidRawMeanings.Contains(cleanMeaning))
        {
            return false;
        }

        return true;
    }

    public static bool IsCleanMcqQuestion(Question q)
    {
        if (string.IsNullOrWhiteSpace(q.Text) ||
            q.Text.StartsWith("[TL:", StringComparison.OrdinalIgnoreCase) ||
            q.Text.StartsWith("[LS:", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (q.Options.Count < 2)
        {
            return false;
        }

        // Loại bỏ các câu hỏi cũ bị sinh từ dòng CSV lệch cột (VD: mang nghĩa "adv", (nghĩa: v), từ "36")
        if (Regex.IsMatch(q.Text, @"(mang nghĩa\s+""|\(nghĩa:\s*)(n|v|adj|adv|prep|pron|conj|det|interj|num|aux|art|noun|verb|adjective|adverb|\d+)(""|[\)\s])", RegexOptions.IgnoreCase) ||
            Regex.IsMatch(q.Text, @"từ\s+""\d+""", RegexOptions.IgnoreCase))
        {
            return false;
        }

        var asksAboutNumbers = q.Text.Contains("số", StringComparison.OrdinalIgnoreCase)
            || q.Text.Contains("bao nhiêu", StringComparison.OrdinalIgnoreCase)
            || q.Text.Contains("tuổi", StringComparison.OrdinalIgnoreCase)
            || q.Text.Contains("giờ", StringComparison.OrdinalIgnoreCase);

        if (!asksAboutNumbers && q.Options.Any(o => Regex.IsMatch(o.Text.Trim(), @"^\d+$") || InvalidRawMeanings.Contains(o.Text.Trim())))
        {
            return false;
        }

        return true;
    }

    public async Task<HarvestBatchResult> HarvestAndPersistWordsAsync(
        int languageId,
        int count = 8,
        string? topicHint = null,
        int? levelId = null,
        CancellationToken ct = default)
    {
        count = Math.Clamp(count, 1, 25);

        var lang = await dbContext.Languages
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == languageId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy ngôn ngữ ID {languageId}.");

        var levels = await dbContext.Levels
            .AsNoTracking()
            .Where(lv => lv.LanguageId == languageId)
            .OrderBy(lv => lv.SortOrder)
            .ToListAsync(ct);

        if (levels.Count == 0)
        {
            throw new InvalidOperationException("Ngôn ngữ này chưa có cấu hình cấp độ (Levels).");
        }

        var topics = await dbContext.Topics
            .AsNoTracking()
            .Where(t => t.LanguageId == languageId)
            .OrderBy(t => t.Id)
            .ToListAsync(ct);

        var existingTermsList = await dbContext.Words
            .AsNoTracking()
            .Where(w => w.LanguageId == languageId)
            .Select(w => w.Term)
            .ToListAsync(ct);

        var existingSet = new HashSet<string>(
            existingTermsList.Select(t => t.Trim().ToLowerInvariant()),
            StringComparer.OrdinalIgnoreCase);

        var normalizedTopic = string.IsNullOrWhiteSpace(topicHint) || topicHint.Equals("random", StringComparison.OrdinalIgnoreCase)
            ? null
            : topicHint.Trim();

        var candidates = new List<(GeneratedVocabItemDto Item, int LevelOffset, int TopicOffset)>();
        var sourcesUsed = new List<string>();

        // 1. Tier 1: AI Gemini Generation (if ApiKey configured)
        if (aiService.HasApiKey)
        {
            var aiItems = await aiService.GenerateVocabularyBatchAsync(
                lang.Code,
                normalizedTopic,
                existingSet.Take(100).ToList(),
                count,
                ct);

            int addedFromAi = 0;
            foreach (var item in aiItems)
            {
                var key = item.Term.Trim().ToLowerInvariant();
                if (string.IsNullOrWhiteSpace(key) || existingSet.Contains(key)) continue;
                existingSet.Add(key);
                candidates.Add((item, Random.Shared.Next(Math.Min(2, levels.Count)), Random.Shared.Next(Math.Max(1, topics.Count))));
                addedFromAi++;
                if (candidates.Count >= count) break;
            }
            if (addedFromAi > 0) sourcesUsed.Add("AI Gemini");
        }

        // 2. Tier 2: Live Web Dictionary + Translation API when custom topic or vault needs expansion
        if (candidates.Count < count && lang.Code == "en")
        {
            var webNeeded = normalizedTopic != null && !IsBuiltInCategory(normalizedTopic)
                ? count - candidates.Count
                : 0;

            if (webNeeded > 0)
            {
                var webItems = await TryHarvestLiveEnglishWebWordsAsync(normalizedTopic!, existingSet, webNeeded, ct);
                if (webItems.Count > 0)
                {
                    foreach (var w in webItems)
                    {
                        candidates.Add((w, 1, 0));
                    }
                    sourcesUsed.Add("Từ điển trực tuyến quốc tế");
                }
            }
        }

        // 3. Tier 3: Built-in EndlessVocabBank + StandardPresets (non-duplicate random sampling)
        if (candidates.Count < count)
        {
            var vaultPool = EndlessVocabBank.Words
                .Where(v => v.LanguageId == languageId && !existingSet.Contains(v.Term.Trim().ToLowerInvariant()))
                .ToList();

            IEnumerable<EndlessVocabBank.VaultWord> orderedVault;
            if (normalizedTopic != null)
            {
                var matched = vaultPool
                    .Where(v => v.Category.Equals(normalizedTopic, StringComparison.OrdinalIgnoreCase) ||
                                v.Meaning.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase) ||
                                v.Term.Contains(normalizedTopic, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(_ => Random.Shared.Next());
                var others = vaultPool
                    .Except(matched)
                    .OrderBy(_ => Random.Shared.Next());
                orderedVault = matched.Concat(others);
            }
            else
            {
                orderedVault = vaultPool.OrderBy(_ => Random.Shared.Next());
            }

            int addedFromVault = 0;
            foreach (var v in orderedVault)
            {
                if (candidates.Count >= count) break;
                var key = v.Term.Trim().ToLowerInvariant();
                if (!existingSet.Add(key)) continue;

                candidates.Add((new GeneratedVocabItemDto
                {
                    Term = v.Term,
                    Reading = v.Reading,
                    Meaning = v.Meaning,
                    PartOfSpeech = v.PartOfSpeech,
                    ExampleSentence = v.Example,
                    ExampleReading = v.ExampleReading,
                    ExampleTranslation = v.ExampleVi
                }, v.LevelOffset, v.TopicOffset));
                addedFromVault++;
            }

            // Also check StandardPresets if any preset words were not imported yet
            if (candidates.Count < count)
            {
                var presetKey = languageId switch { 1 => "oxford", 2 => "hsk", 3 => "jlpt", 4 => "topik", _ => "" };
                if (StandardPresets.Datasets.TryGetValue(presetKey, out var presetDs))
                {
                    foreach (var p in presetDs.Words.OrderBy(_ => Random.Shared.Next()))
                    {
                        if (candidates.Count >= count) break;
                        var key = p.Term.Trim().ToLowerInvariant();
                        if (!existingSet.Add(key)) continue;

                        candidates.Add((new GeneratedVocabItemDto
                        {
                            Term = p.Term,
                            Reading = p.Reading,
                            Meaning = p.Meaning,
                            PartOfSpeech = p.PartOfSpeech,
                            ExampleSentence = p.Example,
                            ExampleReading = p.ExampleReading,
                            ExampleTranslation = p.ExampleVi
                        }, p.LevelOffset, p.TopicOffset));
                        addedFromVault++;
                    }
                }
            }

            if (addedFromVault > 0) sourcesUsed.Add("Kho từ chuẩn mở rộng");
        }

        // 4. Tier 4: Live Web Discovery fallback when built-in vault is fully imported!
        if (candidates.Count < count)
        {
            var remaining = count - candidates.Count;
            var seedTopic = normalizedTopic ?? RandomTopicSeeds[Random.Shared.Next(RandomTopicSeeds.Length)];
            var liveWords = lang.Code switch
            {
                "en" => await TryHarvestLiveEnglishWebWordsAsync(seedTopic, existingSet, remaining, ct),
                "ja" => await TryHarvestLiveJapaneseWebWordsAsync(seedTopic, existingSet, remaining, ct),
                _ => []
            };

            if (liveWords.Count > 0)
            {
                foreach (var lw in liveWords)
                {
                    candidates.Add((lw, 1, 0));
                }
                if (!sourcesUsed.Contains("Từ điển trực tuyến quốc tế"))
                {
                    sourcesUsed.Add("Từ điển trực tuyến quốc tế");
                }
            }
        }

        if (candidates.Count == 0)
        {
            var currentTotal = await dbContext.Words.CountAsync(w => w.LanguageId == languageId, ct);
            return new HarvestBatchResult(
                0,
                currentTotal,
                "Kho từ hiện tại",
                [],
                []);
        }

        // Persist permanently into dbo.Words & dbo.WordExamples
        var newEntities = new List<Word>();
        foreach (var (item, lvlOffset, topicOffset) in candidates)
        {
            var chosenLevel = levelId.HasValue
                ? levels.FirstOrDefault(l => l.Id == levelId.Value) ?? levels[0]
                : levels[Math.Clamp(lvlOffset, 0, levels.Count - 1)];

            var chosenTopic = topics.Count > 0
                ? topics[Math.Clamp(topicOffset, 0, topics.Count - 1)]
                : null;

            var wordEntity = new Word
            {
                LanguageId = languageId,
                LevelId = chosenLevel.Id,
                TopicId = chosenTopic?.Id,
                Term = Truncate(item.Term.Trim(), 200),
                Reading = string.IsNullOrWhiteSpace(item.Reading) ? null : Truncate(item.Reading.Trim(), 200),
                Meaning = Truncate(item.Meaning.Trim(), 500),
                PartOfSpeech = Truncate(string.IsNullOrWhiteSpace(item.PartOfSpeech) ? "noun" : item.PartOfSpeech.Trim(), 50),
                CreatedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrWhiteSpace(item.ExampleSentence))
            {
                wordEntity.Examples.Add(new WordExample
                {
                    Sentence = Truncate(item.ExampleSentence.Trim(), 1000),
                    Reading = string.IsNullOrWhiteSpace(item.ExampleReading) ? null : Truncate(item.ExampleReading.Trim(), 1000),
                    Translation = Truncate(
                        string.IsNullOrWhiteSpace(item.ExampleTranslation) ? item.Meaning.Trim() : item.ExampleTranslation.Trim(),
                        1000)
                });
            }

            dbContext.Words.Add(wordEntity);
            newEntities.Add(wordEntity);
        }

        await dbContext.SaveChangesAsync(ct);

        // Also generate permanent Quiz Questions for newly harvested words so the question bank grows permanently!
        var distractorPoolRaw = await dbContext.Words
            .AsNoTracking()
            .Where(w => w.LanguageId == languageId)
            .Select(w => new { w.Term, w.Meaning })
            .Take(160)
            .ToListAsync(ct);

        var distractorPool = distractorPoolRaw
            .Where(w => IsValidVocabularyWord(w.Term, w.Meaning))
            .ToList();

        foreach (var w in newEntities)
        {
            if (!IsValidVocabularyWord(w.Term, w.Meaning)) continue;

            var qText = $"Nghĩa tiếng Việt của từ \"{w.Term}\" là gì?";
            var wrongMeanings = distractorPool
                .Select(d => d.Meaning)
                .Where(m => !string.Equals(m, w.Meaning, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(_ => Random.Shared.Next())
                .Take(3)
                .ToList();

            if (wrongMeanings.Count >= 2)
            {
                var q = new Question
                {
                    LanguageId = languageId,
                    LevelId = w.LevelId,
                    Text = Truncate(qText, 1000),
                    Explanation = Truncate(
                        string.IsNullOrWhiteSpace(w.Reading)
                            ? $"\"{w.Term}\" ({w.PartOfSpeech}) có nghĩa là \"{w.Meaning}\"."
                            : $"\"{w.Term}\" [{w.Reading}] ({w.PartOfSpeech}) có nghĩa là \"{w.Meaning}\".",
                        1000)
                };
                q.Options.Add(new QuestionOption { Text = Truncate(w.Meaning, 500), IsCorrect = true });
                foreach (var wm in wrongMeanings)
                {
                    q.Options.Add(new QuestionOption { Text = Truncate(wm, 500), IsCorrect = false });
                }
                dbContext.Questions.Add(q);
            }
        }

        await dbContext.SaveChangesAsync(ct);

        var levelMap = levels.ToDictionary(l => l.Id);
        var topicMap = topics.ToDictionary(t => t.Id);
        var totalWordsAfter = await dbContext.Words.CountAsync(w => w.LanguageId == languageId, ct);

        var dtos = newEntities.Select(w => new WordDto
        {
            Id = w.Id,
            LanguageId = w.LanguageId,
            LanguageCode = lang.Code,
            LevelId = w.LevelId,
            LevelCode = levelMap.TryGetValue(w.LevelId, out var lv) ? lv.Code : "",
            LevelName = levelMap.TryGetValue(w.LevelId, out var lv2) ? lv2.Name : "",
            TopicId = w.TopicId,
            TopicName = w.TopicId.HasValue && topicMap.TryGetValue(w.TopicId.Value, out var tp) ? tp.Name : null,
            Term = w.Term,
            Reading = w.Reading,
            Meaning = w.Meaning,
            PartOfSpeech = w.PartOfSpeech,
            ExamplesCount = w.Examples.Count,
            CreatedAt = w.CreatedAt
        }).ToList();

        return new HarvestBatchResult(
            newEntities.Count,
            totalWordsAfter,
            string.Join(" + ", sourcesUsed),
            dtos,
            newEntities.Select(w => w.Id).ToList());
    }

    public async Task<IReadOnlyList<Question>> BuildDynamicListeningQuestionsAsync(
        int languageId,
        int? levelId,
        int questionCount,
        IReadOnlyList<int>? priorityWordIds = null,
        int examSeed = 1,
        CancellationToken ct = default)
    {
        questionCount = Math.Clamp(questionCount, 1, 60);

        var rawLangWords = await dbContext.Words
            .AsNoTracking()
            .Include(w => w.Examples)
            .Where(w => w.LanguageId == languageId)
            .ToListAsync(ct);

        var allLangWords = rawLangWords
            .Where(w => IsValidVocabularyWord(w.Term, w.Meaning))
            .ToList();

        if (allLangWords.Count < 4)
        {
            return [];
        }

        var candidateWords = levelId.HasValue
            ? allLangWords.Where(w => w.LevelId == levelId.Value).ToList()
            : allLangWords;

        if (candidateWords.Count < 4)
        {
            candidateWords = allLangWords;
        }

        var rng = new Random(examSeed * 3571 + languageId * 43 + (levelId ?? 0));

        var prioritySet = priorityWordIds is { Count: > 0 } ? new HashSet<int>(priorityWordIds) : [];
        var prioritized = candidateWords
            .Where(w => prioritySet.Contains(w.Id))
            .OrderBy(_ => rng.Next())
            .ToList();

        var rest = candidateWords
            .Where(w => !prioritySet.Contains(w.Id))
            .OrderBy(_ => rng.Next())
            .ToList();

        var pool = prioritized.Concat(rest).ToList();
        if (pool.Count < 4) pool = allLangWords;

        var generatedTexts = new List<(Word Target, string Text, string Explanation, string CorrectText, List<string> Distractors)>();
        var seenTexts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        int round = 0;
        while (generatedTexts.Count < questionCount && round < 4)
        {
            foreach (var word in pool)
            {
                if (generatedTexts.Count >= questionCount) break;

                var style = (round + word.Id) % 3;
                string qText;
                string audioScript;
                string correctText;
                List<string> distractors;

                var example = word.Examples.FirstOrDefault();

                if (style == 1 && example != null && !string.IsNullOrWhiteSpace(example.Sentence))
                {
                    audioScript = example.Sentence.Trim();
                    qText = $"[LS:SENT|{audioScript}] Nghe đoạn câu hoàn chỉnh và chọn từ xuất hiện trong câu (nghĩa câu: \"{example.Translation}\"):";
                    correctText = word.Term;
                    distractors = allLangWords
                        .Where(x => x.Id != word.Id && !string.Equals(x.Term, word.Term, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.Term)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(_ => rng.Next())
                        .Take(3)
                        .ToList();
                }
                else if (style == 2)
                {
                    audioScript = word.Term.Trim();
                    var posLabel = !string.IsNullOrWhiteSpace(word.PartOfSpeech) ? $" ({word.PartOfSpeech})" : "";
                    qText = $"[LS:TERM|{audioScript}] Nghe phát âm và chọn cách viết chính xác của từ (nghĩa: \"{word.Meaning}\"{posLabel}):";
                    correctText = word.Term;
                    distractors = allLangWords
                        .Where(x => x.Id != word.Id && !string.Equals(x.Term, word.Term, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.Term)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(_ => rng.Next())
                        .Take(3)
                        .ToList();
                }
                else
                {
                    audioScript = word.Term.Trim();
                    qText = $"[LS:WORD|{audioScript}] Nghe phát âm từ vựng và chọn nghĩa tiếng Việt chính xác:";
                    correctText = word.Meaning;
                    distractors = allLangWords
                        .Where(x => x.Id != word.Id && !string.Equals(x.Meaning, word.Meaning, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.Meaning)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(_ => rng.Next())
                        .Take(3)
                        .ToList();
                }

                if (distractors.Count < 2) continue;
                if (!seenTexts.Add(qText)) continue;

                var explanation = string.IsNullOrWhiteSpace(word.Reading)
                    ? $"[Phát âm: \"{audioScript}\"] • \"{word.Term}\" ({word.PartOfSpeech ?? "từ vựng"}) có nghĩa là \"{word.Meaning}\"."
                    : $"[Phát âm: \"{audioScript}\"] • \"{word.Term}\" [{word.Reading}] ({word.PartOfSpeech ?? "từ vựng"}) có nghĩa là \"{word.Meaning}\".";

                generatedTexts.Add((word, Truncate(qText, 1000), Truncate(explanation, 1000), Truncate(correctText, 500), distractors));
            }
            round++;
        }

        var textLookupSet = generatedTexts.Select(g => g.Text).Distinct().ToList();
        var existingRows = await dbContext.Questions
            .Include(q => q.Options)
            .Where(q => q.LanguageId == languageId && textLookupSet.Contains(q.Text))
            .ToListAsync(ct);

        var existingQuestions = existingRows
            .Where(q => q.Options.Count >= 2 && q.Options.Any(o => o.IsCorrect))
            .GroupBy(q => q.Text)
            .ToDictionary(g => g.Key, g => g.First());

        var candidateQuestions = new List<Question>();
        var newQuestionsToSave = new List<Question>();

        foreach (var g in generatedTexts)
        {
            if (existingQuestions.TryGetValue(g.Text, out var existingQ) && existingQ.Options.Count >= 2)
            {
                candidateQuestions.Add(existingQ);
            }
            else
            {
                var newQ = new Question
                {
                    LanguageId = languageId,
                    LevelId = g.Target.LevelId,
                    Text = g.Text,
                    Explanation = g.Explanation
                };
                newQ.Options.Add(new QuestionOption { Text = g.CorrectText, IsCorrect = true });
                foreach (var d in g.Distractors)
                {
                    newQ.Options.Add(new QuestionOption { Text = Truncate(d, 500), IsCorrect = false });
                }
                dbContext.Questions.Add(newQ);
                newQuestionsToSave.Add(newQ);
                candidateQuestions.Add(newQ);
                existingQuestions[g.Text] = newQ;
            }
        }

        if (newQuestionsToSave.Count > 0)
        {
            await dbContext.SaveChangesAsync(ct);
        }

        return candidateQuestions;
    }

    public async Task<IReadOnlyList<Question>> BuildDynamicVocabQuizQuestionsAsync(
        int languageId,
        int? levelId,
        int questionCount,
        IReadOnlyList<int>? priorityWordIds = null,
        int examSeed = 1,
        CancellationToken ct = default)
    {
        questionCount = Math.Clamp(questionCount, 1, 60);

        var rawLangWords = await dbContext.Words
            .AsNoTracking()
            .Include(w => w.Examples)
            .Where(w => w.LanguageId == languageId)
            .ToListAsync(ct);

        var allLangWords = rawLangWords
            .Where(w => IsValidVocabularyWord(w.Term, w.Meaning))
            .ToList();

        if (allLangWords.Count < 4)
        {
            return [];
        }

        var candidateWords = levelId.HasValue
            ? allLangWords.Where(w => w.LevelId == levelId.Value).ToList()
            : allLangWords;

        if (candidateWords.Count < 4)
        {
            candidateWords = allLangWords;
        }

        var rng = new Random(examSeed * 7919 + languageId * 37 + (levelId ?? 0));

        var prioritySet = priorityWordIds is { Count: > 0 } ? new HashSet<int>(priorityWordIds) : [];
        var prioritized = candidateWords
            .Where(w => prioritySet.Contains(w.Id))
            .OrderBy(_ => rng.Next())
            .ToList();

        var rest = candidateWords
            .Where(w => !prioritySet.Contains(w.Id))
            .OrderBy(_ => rng.Next())
            .ToList();

        var pool = prioritized.Concat(rest).ToList();
        if (pool.Count < 4) pool = allLangWords;

        // Check existing questions by exact Text to reuse or persist new dynamic templates permanently
        var candidateQuestions = new List<Question>();
        var generatedTexts = new List<(Word Target, int Style, string Text, string Explanation, string CorrectText, List<string> Distractors)>();
        var seenTexts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        int round = 0;
        while (generatedTexts.Count < questionCount && round < 4)
        {
            foreach (var word in pool)
            {
                if (generatedTexts.Count >= questionCount) break;

                // Choose a varied style:
                // 0 = Term -> Meaning, 1 = Meaning -> Term, 2 = Reading -> Term, 3 = Example Blank -> Term
                var availableStyles = new List<int> { 0, 1 };
                if (!string.IsNullOrWhiteSpace(word.Reading)) availableStyles.Add(2);

                var example = word.Examples.FirstOrDefault();
                string? blankedSentence = null;
                if (example != null && example.Sentence.Contains(word.Term, StringComparison.OrdinalIgnoreCase))
                {
                    blankedSentence = Regex.Replace(
                        example.Sentence,
                        Regex.Escape(word.Term),
                        "______",
                        RegexOptions.IgnoreCase);
                    if (blankedSentence != example.Sentence)
                    {
                        availableStyles.Add(3);
                    }
                }

                var style = availableStyles[(round + rng.Next(availableStyles.Count)) % availableStyles.Count];
                string qText;
                string correctText;
                List<string> distractors;

                if (style == 0)
                {
                    var readingHint = !string.IsNullOrWhiteSpace(word.Reading) ? $" ({word.Reading})" : "";
                    qText = $"Nghĩa tiếng Việt của từ \"{word.Term}\"{readingHint} là gì?";
                    correctText = word.Meaning;
                    distractors = allLangWords
                        .Where(x => x.Id != word.Id && !string.Equals(x.Meaning, word.Meaning, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.Meaning)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(_ => rng.Next())
                        .Take(3)
                        .ToList();
                }
                else if (style == 2)
                {
                    qText = $"Phiên âm \"{word.Reading}\" tương ứng với từ vựng nào dưới đây (nghĩa: {word.Meaning})?";
                    correctText = word.Term;
                    distractors = allLangWords
                        .Where(x => x.Id != word.Id && !string.Equals(x.Term, word.Term, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.Term)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(_ => rng.Next())
                        .Take(3)
                        .ToList();
                }
                else if (style == 3 && blankedSentence != null && example != null)
                {
                    qText = $"Điền từ thích hợp vào chỗ trống: \"{blankedSentence}\" ({example.Translation})";
                    correctText = word.Term;
                    distractors = allLangWords
                        .Where(x => x.Id != word.Id && !string.Equals(x.Term, word.Term, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.Term)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(_ => rng.Next())
                        .Take(3)
                        .ToList();
                }
                else
                {
                    qText = $"Từ vựng nào dưới đây mang nghĩa \"{word.Meaning}\"?";
                    correctText = word.Term;
                    distractors = allLangWords
                        .Where(x => x.Id != word.Id && !string.Equals(x.Term, word.Term, StringComparison.OrdinalIgnoreCase))
                        .Select(x => x.Term)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(_ => rng.Next())
                        .Take(3)
                        .ToList();
                }

                if (distractors.Count < 2) continue;
                if (!seenTexts.Add(qText)) continue;

                var explanation = string.IsNullOrWhiteSpace(word.Reading)
                    ? $"\"{word.Term}\" ({word.PartOfSpeech ?? "từ vựng"}) nghĩa là \"{word.Meaning}\"."
                    : $"\"{word.Term}\" [{word.Reading}] ({word.PartOfSpeech ?? "từ vựng"}) nghĩa là \"{word.Meaning}\".";

                generatedTexts.Add((word, style, Truncate(qText, 1000), Truncate(explanation, 1000), Truncate(correctText, 500), distractors));
            }
            round++;
        }

        var textLookupSet = generatedTexts.Select(g => g.Text).Distinct().ToList();
        var existingRows = await dbContext.Questions
            .Include(q => q.Options)
            .Where(q => q.LanguageId == languageId && textLookupSet.Contains(q.Text))
            .ToListAsync(ct);

        var existingQuestions = existingRows
            .Where(IsCleanMcqQuestion)
            .GroupBy(q => q.Text)
            .ToDictionary(g => g.Key, g => g.First());

        var newQuestionsToSave = new List<Question>();
        foreach (var g in generatedTexts)
        {
            if (existingQuestions.TryGetValue(g.Text, out var existingQ) && existingQ.Options.Count >= 2)
            {
                candidateQuestions.Add(existingQ);
            }
            else
            {
                var newQ = new Question
                {
                    LanguageId = languageId,
                    LevelId = g.Target.LevelId,
                    Text = g.Text,
                    Explanation = g.Explanation
                };
                newQ.Options.Add(new QuestionOption { Text = g.CorrectText, IsCorrect = true });
                foreach (var d in g.Distractors)
                {
                    newQ.Options.Add(new QuestionOption { Text = Truncate(d, 500), IsCorrect = false });
                }
                dbContext.Questions.Add(newQ);
                newQuestionsToSave.Add(newQ);
                candidateQuestions.Add(newQ);
                existingQuestions[g.Text] = newQ;
            }
        }

        if (newQuestionsToSave.Count > 0)
        {
            await dbContext.SaveChangesAsync(ct);
        }

        return candidateQuestions;
    }

    public async Task<IReadOnlyList<Question>> BuildDynamicWrittenQuestionsAsync(
        int languageId,
        int? levelId,
        int questionCount,
        IReadOnlyList<int>? priorityWordIds = null,
        IReadOnlyCollection<string>? excludeTargetTerms = null,
        int examSeed = 1,
        CancellationToken ct = default)
    {
        questionCount = Math.Clamp(questionCount, 1, 30);

        var rawLangWords = await dbContext.Words
            .AsNoTracking()
            .Include(w => w.Examples)
            .Where(w => w.LanguageId == languageId)
            .ToListAsync(ct);

        var allLangWords = rawLangWords
            .Where(w => IsValidVocabularyWord(w.Term, w.Meaning))
            .ToList();

        if (allLangWords.Count == 0)
        {
            return [];
        }

        var rng = new Random(examSeed * 4999 + languageId * 53 + (levelId ?? 0));

        var excludeSet = excludeTargetTerms is { Count: > 0 }
            ? new HashSet<string>(excludeTargetTerms, StringComparer.OrdinalIgnoreCase)
            : [];

        var levelFiltered = levelId.HasValue
            ? allLangWords.Where(w => w.LevelId == levelId.Value).ToList()
            : allLangWords;

        if (levelFiltered.Count < 4)
        {
            levelFiltered = allLangWords;
        }

        var nonExcluded = levelFiltered
            .Where(w => !excludeSet.Contains(w.Term.Trim()))
            .ToList();

        var pool = nonExcluded.Count >= 4 ? nonExcluded : levelFiltered;

        var prioritySet = priorityWordIds is { Count: > 0 } ? new HashSet<int>(priorityWordIds) : [];
        var prioritized = pool
            .Where(w => prioritySet.Contains(w.Id))
            .OrderBy(_ => rng.Next())
            .ToList();

        var rest = pool
            .Where(w => !prioritySet.Contains(w.Id))
            .OrderBy(_ => rng.Next())
            .ToList();

        var combinedPool = prioritized.Concat(rest).ToList();
        var targetWords = new List<Word>();
        while (targetWords.Count < questionCount && combinedPool.Count > 0)
        {
            foreach (var w in combinedPool)
            {
                if (targetWords.Count >= questionCount) break;
                targetWords.Add(w);
            }
        }
        int maxSentenceTasks = Math.Clamp(questionCount / 3, 1, 3);
        int sentenceTasksAssigned = 0;

        var generatedItems = new List<(Word Target, string Text, string Explanation, string ExpectedTerm)>();

        for (int idx = 0; idx < targetWords.Count; idx++)
        {
            var word = targetWords[idx];
            var example = word.Examples.FirstOrDefault();
            var posLabel = !string.IsNullOrWhiteSpace(word.PartOfSpeech) ? $" ({word.PartOfSpeech})" : "";
            var readingInfo = !string.IsNullOrWhiteSpace(word.Reading) ? $" [{word.Reading}]" : "";

            string? blankedSentence = null;
            if (example != null && example.Sentence.Contains(word.Term, StringComparison.OrdinalIgnoreCase))
            {
                blankedSentence = Regex.Replace(
                    example.Sentence,
                    Regex.Escape(word.Term),
                    "______",
                    RegexOptions.IgnoreCase);
                if (blankedSentence == example.Sentence)
                {
                    blankedSentence = null;
                }
            }

            string qText;
            string explanation;

            // Đặt câu hoàn chỉnh (ưu tiên ở các câu cuối của phần Tự luận)
            bool assignSentenceTask = sentenceTasksAssigned < maxSentenceTasks &&
                (idx >= targetWords.Count - maxSentenceTasks || questionCount == 1 && Random.Shared.Next(2) == 0);

            if (assignSentenceTask)
            {
                sentenceTasksAssigned++;
                qText = $"[TL:SENTENCE] Hãy tự đặt 1 câu hoàn chỉnh bằng ngoại ngữ có sử dụng từ \"{word.Term}\"{readingInfo} (nghĩa: \"{word.Meaning}\"):";
                explanation = example != null
                    ? $"Từ bắt buộc: \"{word.Term}\"{readingInfo} ({word.Meaning}). Câu mẫu tham khảo: \"{example.Sentence}\" — {example.Translation}"
                    : $"Từ bắt buộc: \"{word.Term}\"{readingInfo} ({word.Meaning}).";
            }
            else if (blankedSentence != null && example != null && idx % 2 == 1)
            {
                qText = $"[TL:CLOZE] Điền từ vựng chính xác vào chỗ trống: \"{blankedSentence}\" (Nghĩa câu: {example.Translation})";
                explanation = $"Đáp án đúng: \"{word.Term}\"{readingInfo} ({word.Meaning}). Câu đầy đủ: \"{example.Sentence}\"";
            }
            else
            {
                qText = $"[TL:TERM] Hãy viết chính xác từ vựng ngoại ngữ mang nghĩa \"{word.Meaning}\"{posLabel}:";
                explanation = example != null
                    ? $"Đáp án chuẩn: \"{word.Term}\"{readingInfo}{posLabel} nghĩa là \"{word.Meaning}\". Ví dụ: \"{example.Sentence}\""
                    : $"Đáp án chuẩn: \"{word.Term}\"{readingInfo}{posLabel} nghĩa là \"{word.Meaning}\".";
            }

            generatedItems.Add((word, Truncate(qText, 1000), Truncate(explanation, 1000), Truncate(word.Term.Trim(), 500)));
        }

        var lookupTexts = generatedItems.Select(g => g.Text).Distinct().ToList();
        var existingWrittenRows = await dbContext.Questions
            .Include(q => q.Options)
            .Where(q => q.LanguageId == languageId && lookupTexts.Contains(q.Text))
            .ToListAsync(ct);

        var existingMap = existingWrittenRows
            .Where(q => q.Options.Any(o => o.IsCorrect))
            .GroupBy(q => q.Text)
            .ToDictionary(g => g.Key, g => g.First());

        var resultQuestions = new List<Question>();
        var toPersist = new List<Question>();

        foreach (var item in generatedItems)
        {
            if (existingMap.TryGetValue(item.Text, out var existingQ))
            {
                resultQuestions.Add(existingQ);
            }
            else
            {
                var newQ = new Question
                {
                    LanguageId = languageId,
                    LevelId = item.Target.LevelId,
                    Text = item.Text,
                    Explanation = item.Explanation
                };
                newQ.Options.Add(new QuestionOption
                {
                    Text = item.ExpectedTerm,
                    IsCorrect = true
                });
                dbContext.Questions.Add(newQ);
                toPersist.Add(newQ);
                resultQuestions.Add(newQ);
                existingMap[item.Text] = newQ;
            }
        }

        if (toPersist.Count > 0)
        {
            await dbContext.SaveChangesAsync(ct);
        }

        return resultQuestions;
    }

    private async Task<List<GeneratedVocabItemDto>> TryHarvestLiveEnglishWebWordsAsync(
        string topicSeed,
        HashSet<string> existingSet,
        int needed,
        CancellationToken ct)
    {
        var results = new List<GeneratedVocabItemDto>();
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(4.5));

            var dmUrl = $"https://api.datamuse.com/words?ml={Uri.EscapeDataString(topicSeed)}&md=p&max=35";
            var dmDoc = await httpClient.GetFromJsonAsync<JsonElement>(dmUrl, cts.Token);
            if (dmDoc.ValueKind != JsonValueKind.Array) return results;

            var rawWords = new List<string>();
            foreach (var el in dmDoc.EnumerateArray())
            {
                if (!el.TryGetProperty("word", out var wProp)) continue;
                var w = wProp.GetString()?.Trim();
                if (string.IsNullOrWhiteSpace(w) || w.Contains(' ') || w.Contains('-') || w.Length < 4 || w.Length > 14) continue;
                if (!Regex.IsMatch(w, "^[a-zA-Z]+$")) continue;
                if (existingSet.Contains(w.ToLowerInvariant())) continue;
                rawWords.Add(w);
            }

            foreach (var rawWord in rawWords.OrderBy(_ => Random.Shared.Next()).Take(needed + 3))
            {
                if (results.Count >= needed) break;

                var termCapitalized = char.ToUpperInvariant(rawWord[0]) + rawWord[1..].ToLowerInvariant();
                string? phonetic = null;
                string pos = "noun";
                string exampleEn = $"Let's learn how to use the word \"{rawWord.ToLowerInvariant()}\" in daily life.";

                // Enrich with Free Dictionary API (IPA + Example)
                try
                {
                    var dictUrl = $"https://api.dictionaryapi.dev/api/v2/entries/en/{Uri.EscapeDataString(rawWord.ToLowerInvariant())}";
                    var dictResp = await httpClient.GetAsync(dictUrl, cts.Token);
                    if (dictResp.IsSuccessStatusCode)
                    {
                        var dictJson = await dictResp.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cts.Token);
                        if (dictJson.ValueKind == JsonValueKind.Array && dictJson.GetArrayLength() > 0)
                        {
                            var first = dictJson[0];
                            if (first.TryGetProperty("phonetic", out var ph) && !string.IsNullOrWhiteSpace(ph.GetString()))
                            {
                                phonetic = ph.GetString();
                            }
                            if (first.TryGetProperty("meanings", out var meanings) && meanings.ValueKind == JsonValueKind.Array && meanings.GetArrayLength() > 0)
                            {
                                var m0 = meanings[0];
                                if (m0.TryGetProperty("partOfSpeech", out var posProp) && !string.IsNullOrWhiteSpace(posProp.GetString()))
                                {
                                    pos = posProp.GetString()!;
                                }
                                if (m0.TryGetProperty("definitions", out var defs) && defs.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var d in defs.EnumerateArray())
                                    {
                                        if (d.TryGetProperty("example", out var exProp) && !string.IsNullOrWhiteSpace(exProp.GetString()))
                                        {
                                            exampleEn = exProp.GetString()!;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // Continue even if phonetic lookup fails
                }

                // Translate meaning to Vietnamese via MyMemory public endpoint
                var viMeaning = await TryTranslateToVietnameseAsync(termCapitalized, "en", cts.Token);
                if (string.IsNullOrWhiteSpace(viMeaning) ||
                    string.Equals(viMeaning, termCapitalized, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var exampleVi = await TryTranslateToVietnameseAsync(exampleEn, "en", cts.Token) ?? $"Ví dụ sử dụng từ {termCapitalized} ({viMeaning}).";

                existingSet.Add(rawWord.ToLowerInvariant());
                results.Add(new GeneratedVocabItemDto
                {
                    Term = termCapitalized,
                    Reading = phonetic,
                    Meaning = viMeaning.ToLowerInvariant(),
                    PartOfSpeech = pos,
                    ExampleSentence = exampleEn,
                    ExampleReading = null,
                    ExampleTranslation = exampleVi
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Không thể kết nối nguồn từ điển trực tuyến, sử dụng kho nội bộ.");
        }

        return results;
    }

    private async Task<List<GeneratedVocabItemDto>> TryHarvestLiveJapaneseWebWordsAsync(
        string topicSeed,
        HashSet<string> existingSet,
        int needed,
        CancellationToken ct)
    {
        var results = new List<GeneratedVocabItemDto>();
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(4.5));

            var jishoUrl = $"https://jisho.org/api/v1/search/words?keyword={Uri.EscapeDataString(topicSeed)}";
            var doc = await httpClient.GetFromJsonAsync<JsonElement>(jishoUrl, cts.Token);
            if (!doc.TryGetProperty("data", out var dataArr) || dataArr.ValueKind != JsonValueKind.Array)
                return results;

            foreach (var entry in dataArr.EnumerateArray().OrderBy(_ => Random.Shared.Next()).Take(needed + 4))
            {
                if (results.Count >= needed) break;
                if (!entry.TryGetProperty("japanese", out var jpArr) || jpArr.GetArrayLength() == 0) continue;
                var jp0 = jpArr[0];
                var word = jp0.TryGetProperty("word", out var wProp) ? wProp.GetString() : null;
                var reading = jp0.TryGetProperty("reading", out var rProp) ? rProp.GetString() : null;
                var term = !string.IsNullOrWhiteSpace(word) ? word! : reading;
                if (string.IsNullOrWhiteSpace(term) || existingSet.Contains(term.ToLowerInvariant())) continue;

                var viMeaning = await TryTranslateToVietnameseAsync(term, "ja", cts.Token);
                if (string.IsNullOrWhiteSpace(viMeaning) || string.Equals(viMeaning, term, StringComparison.OrdinalIgnoreCase))
                    continue;

                existingSet.Add(term.ToLowerInvariant());
                results.Add(new GeneratedVocabItemDto
                {
                    Term = term,
                    Reading = reading,
                    Meaning = viMeaning.ToLowerInvariant(),
                    PartOfSpeech = "noun",
                    ExampleSentence = $"{term}について勉強しています。",
                    ExampleReading = reading != null ? $"{reading} ni tsuite benkyō shiteimasu." : null,
                    ExampleTranslation = $"Tôi đang tìm hiểu về {viMeaning.ToLowerInvariant()}."
                });
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Không thể kết nối Jisho trực tuyến, sử dụng kho nội bộ.");
        }

        return results;
    }

    private async Task<string?> TryTranslateToVietnameseAsync(string text, string sourceLang, CancellationToken ct)
    {
        try
        {
            var url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair={sourceLang}|vi";
            var json = await httpClient.GetFromJsonAsync<JsonElement>(url, ct);
            if (json.TryGetProperty("responseData", out var rd) &&
                rd.TryGetProperty("translatedText", out var tt))
            {
                var translated = tt.GetString()?.Trim();
                if (!string.IsNullOrWhiteSpace(translated) && !translated.Contains("MYMEMORY WARNING", StringComparison.OrdinalIgnoreCase))
                {
                    return translated;
                }
            }
        }
        catch
        {
            // Ignore network errors
        }
        return null;
    }

    private static bool IsBuiltInCategory(string topic)
    {
        return topic.Equals("technology", StringComparison.OrdinalIgnoreCase)
            || topic.Equals("travel", StringComparison.OrdinalIgnoreCase)
            || topic.Equals("business", StringComparison.OrdinalIgnoreCase)
            || topic.Equals("food", StringComparison.OrdinalIgnoreCase)
            || topic.Equals("health", StringComparison.OrdinalIgnoreCase)
            || topic.Equals("culture", StringComparison.OrdinalIgnoreCase);
    }

    private static string Truncate(string value, int maxLen)
        => value.Length <= maxLen ? value : value[..maxLen];
}

