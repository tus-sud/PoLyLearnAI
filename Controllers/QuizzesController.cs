using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Data;
using PolyLearnAI.Api.Models.DTOs.Ai;
using PolyLearnAI.Api.Models.DTOs.Common;
using PolyLearnAI.Api.Models.DTOs.Quiz;
using PolyLearnAI.Api.Models.Entities;
using PolyLearnAI.Api.Services;
using PolyLearnAI.Api.Services.Implementations;
using PolyLearnAI.Api.Services.Interfaces;

namespace PolyLearnAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuizzesController(
    AppDbContext dbContext,
    IVocabHarvesterService vocabHarvester,
    IAiService aiService) : ControllerBase
{
    /// <summary>
    /// Tạo bài kiểm tra (Kết hợp Trắc nghiệm + Tự luận, Chỉ Trắc nghiệm, hoặc Chỉ Tự luận)
    /// kèm chế độ tự động lấy thêm từ mới lưu vĩnh viễn vào Cơ sở dữ liệu.
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<QuizSessionDto>> GenerateQuiz(
        [FromBody] GenerateQuizRequest request,
        CancellationToken ct)
    {
        var userId = GetRequiredUserId();
        var count = Math.Clamp(request.QuestionCount, 3, 110);

        var lang = await dbContext.Languages.FirstOrDefaultAsync(l => l.Id == request.LanguageId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy ngôn ngữ ID {request.LanguageId}.");

        HarvestBatchResult? harvestBatch = null;
        if (request.HarvestNewWords && !request.LessonId.HasValue)
        {
            var toHarvest = Math.Clamp(request.NewWordsCount, 2, Math.Max(4, count));
            harvestBatch = await vocabHarvester.HarvestAndPersistWordsAsync(
                request.LanguageId,
                toHarvest,
                request.TopicHint,
                request.LevelId,
                ct);
        }

        var examFormat = NormalizeExamFormat(request.ExamFormat);
        var mode = (request.Mode ?? "vocab_random").Trim().ToLowerInvariant();

        bool isOfficialMock = examFormat == "official_mock" || (request.ListeningCount.HasValue && request.ListeningCount.Value > 0);
        int examSeed = request.ExamNumber ?? 1;
        int durationMinutes = request.DurationMinutes ?? 65;

        int listeningQuota = 0;
        int mcqQuota = 0;
        int writtenQuota = 0;

        if (isOfficialMock)
        {
            examFormat = "official_mock";
            listeningQuota = Math.Clamp(request.ListeningCount ?? 40, 0, 50);
            mcqQuota = Math.Clamp(request.ReadingCount ?? 30, 0, 50);
            writtenQuota = Math.Clamp(request.WritingCount ?? 10, 0, 30);
            count = listeningQuota + mcqQuota + writtenQuota;
        }
        else if (request.LessonId.HasValue || examFormat == "mcq")
        {
            mcqQuota = count;
            writtenQuota = 0;
            examFormat = "mcq";
        }
        else if (examFormat == "written")
        {
            mcqQuota = 0;
            writtenQuota = count;
        }
        else
        {
            writtenQuota = count <= 4 ? 1 : Math.Max(2, count * 2 / 5);
            mcqQuota = count - writtenQuota;
        }

        var listeningQuestions = new List<Question>();
        var mcqQuestions = new List<Question>();
        var writtenQuestions = new List<Question>();

        // 1. Sinh phần Nghe (Listening) nếu là đề thi chính thức
        if (listeningQuota > 0)
        {
            var dynListening = await vocabHarvester.BuildDynamicListeningQuestionsAsync(
                request.LanguageId,
                request.LevelId,
                listeningQuota,
                harvestBatch?.InsertedWordIds,
                examSeed,
                ct);
            listeningQuestions.AddRange(dynListening.Take(listeningQuota));
        }

        // 2. Sinh phần Đọc / Trắc nghiệm (Reading / MCQ)
        if (mcqQuota > 0)
        {
            if (isOfficialMock)
            {
                var dynQuestions = await vocabHarvester.BuildDynamicVocabQuizQuestionsAsync(
                    request.LanguageId,
                    request.LevelId,
                    mcqQuota,
                    harvestBatch?.InsertedWordIds,
                    examSeed,
                    ct);
                mcqQuestions.AddRange(dynQuestions.Take(mcqQuota));
            }
            else if (request.LessonId.HasValue)
            {
                var rawLessonQuestions = await dbContext.Questions
                    .AsNoTracking()
                    .Include(q => q.Options)
                    .Where(q => q.LanguageId == request.LanguageId
                        && q.LessonId == request.LessonId.Value
                        && !q.Text.StartsWith("[TL:")
                        && !q.Text.StartsWith("[LS:"))
                    .OrderBy(_ => EF.Functions.Random())
                    .Take(mcqQuota * 2)
                    .ToListAsync(ct);

                mcqQuestions = rawLessonQuestions
                    .Where(VocabHarvesterService.IsCleanMcqQuestion)
                    .Take(mcqQuota)
                    .ToList();
            }
            else if (mode == "vocab_random")
            {
                var dynQuestions = await vocabHarvester.BuildDynamicVocabQuizQuestionsAsync(
                    request.LanguageId,
                    request.LevelId,
                    mcqQuota,
                    harvestBatch?.InsertedWordIds,
                    examSeed,
                    ct);
                mcqQuestions.AddRange(dynQuestions);
            }
            else if (mode == "mixed")
            {
                var vocabPart = Math.Max(1, (mcqQuota + 1) / 2);
                var dynQuestions = await vocabHarvester.BuildDynamicVocabQuizQuestionsAsync(
                    request.LanguageId,
                    request.LevelId,
                    vocabPart,
                    harvestBatch?.InsertedWordIds,
                    examSeed,
                    ct);
                mcqQuestions.AddRange(dynQuestions);
            }

            // Bổ sung từ ngân hàng câu hỏi sạch nếu chọn "bank", "mixed" hoặc còn thiếu câu trắc nghiệm
            if (mcqQuestions.Count < mcqQuota && !request.LessonId.HasValue)
            {
                var existingIds = mcqQuestions.Select(q => q.Id).ToList();
                var needed = mcqQuota - mcqQuestions.Count;

                var bankQuery = dbContext.Questions
                    .AsNoTracking()
                    .Include(q => q.Options)
                    .Where(q => q.LanguageId == request.LanguageId
                        && !existingIds.Contains(q.Id)
                        && !q.Text.StartsWith("[TL:")
                        && !q.Text.StartsWith("[LS:"));

                if (request.LevelId.HasValue)
                {
                    bankQuery = bankQuery.Where(q => q.LevelId == request.LevelId.Value);
                }

                var rawBank = await bankQuery
                    .OrderBy(_ => EF.Functions.Random())
                    .Take(needed * 3)
                    .ToListAsync(ct);

                var cleanBank = rawBank
                    .Where(VocabHarvesterService.IsCleanMcqQuestion)
                    .Take(needed)
                    .ToList();

                mcqQuestions.AddRange(cleanBank);

                if (mcqQuestions.Count < mcqQuota)
                {
                    var fallbackDyn = await vocabHarvester.BuildDynamicVocabQuizQuestionsAsync(
                        request.LanguageId,
                        request.LevelId,
                        mcqQuota - mcqQuestions.Count,
                        harvestBatch?.InsertedWordIds,
                        examSeed,
                        ct);

                    foreach (var fq in fallbackDyn)
                    {
                        if (mcqQuestions.All(x => x.Id != fq.Id))
                        {
                            mcqQuestions.Add(fq);
                        }
                    }
                }
            }

            if (!isOfficialMock)
            {
                mcqQuestions = mcqQuestions
                    .OrderBy(_ => Random.Shared.Next())
                    .Take(mcqQuota)
                    .ToList();
            }
            else
            {
                mcqQuestions = mcqQuestions.Take(mcqQuota).ToList();
            }
        }

        // 3. Sinh phần Viết / Tự luận (Writing)
        if (writtenQuota > 0)
        {
            var usedTerms = mcqQuestions
                .Concat(listeningQuestions)
                .SelectMany(q => q.Options.Where(o => o.IsCorrect).Select(o => o.Text))
                .ToList();

            var dynWritten = await vocabHarvester.BuildDynamicWrittenQuestionsAsync(
                request.LanguageId,
                request.LevelId,
                writtenQuota,
                harvestBatch?.InsertedWordIds,
                usedTerms,
                examSeed,
                ct);

            writtenQuestions.AddRange(dynWritten.Take(writtenQuota));
        }

        // Kết hợp 3 phần: Nghe (1..N) -> Đọc (N+1..M) -> Viết (M+1..Total)
        var allSelectedQuestions = new List<Question>(listeningQuestions.Count + mcqQuestions.Count + writtenQuestions.Count);
        allSelectedQuestions.AddRange(listeningQuestions);
        allSelectedQuestions.AddRange(mcqQuestions);
        allSelectedQuestions.AddRange(writtenQuestions);

        if (allSelectedQuestions.Count == 0)
        {
            throw new BadHttpRequestException("Không có đủ dữ liệu câu hỏi hoặc từ vựng cho tiêu chí đã chọn.");
        }

        var totalWordsInBank = harvestBatch?.TotalWordsInLanguage
            ?? await dbContext.Words.CountAsync(w => w.LanguageId == request.LanguageId, ct);

        var level = request.LevelId.HasValue
            ? await dbContext.Levels.FirstOrDefaultAsync(lv => lv.Id == request.LevelId.Value, ct)
            : null;

        var attempt = new QuizAttempt
        {
            UserId = userId,
            LanguageId = request.LanguageId,
            LevelId = request.LevelId,
            TotalQuestions = allSelectedQuestions.Count,
            Score = 0,
            StartedAt = DateTime.UtcNow
        };

        dbContext.QuizAttempts.Add(attempt);
        await dbContext.SaveChangesAsync(ct);

        var questionDtos = allSelectedQuestions.Select(q =>
        {
            if (TryParseListeningQuestion(q.Text, out var lsSubtype, out var audioScript, out var cleanLsPrompt))
            {
                return new QuizQuestionDto
                {
                    QuestionId = q.Id,
                    Text = cleanLsPrompt,
                    QuestionType = "mcq",
                    Section = "listening",
                    AudioText = audioScript,
                    Options = q.Options
                        .OrderBy(_ => Random.Shared.Next())
                        .Select(o => new QuizOptionDto
                        {
                            Id = o.Id,
                            Text = o.Text
                        }).ToList()
                };
            }

            if (TryParseWrittenQuestion(q.Text, out var subtype, out var cleanPrompt))
            {
                var expectedTerm = q.Options.FirstOrDefault(o => o.IsCorrect)?.Text ?? string.Empty;
                return new QuizQuestionDto
                {
                    QuestionId = q.Id,
                    Text = cleanPrompt,
                    QuestionType = "written",
                    Section = "writing",
                    WrittenSubtype = subtype,
                    TargetTerm = subtype == "make_sentence" ? expectedTerm : null,
                    Hint = BuildWrittenHint(subtype, expectedTerm),
                    Options = []
                };
            }

            return new QuizQuestionDto
            {
                QuestionId = q.Id,
                Text = q.Text,
                QuestionType = "mcq",
                Section = "reading",
                Options = q.Options
                    .OrderBy(_ => Random.Shared.Next())
                    .Select(o => new QuizOptionDto
                    {
                        Id = o.Id,
                        Text = o.Text
                    }).ToList()
            };
        }).ToList();

        var session = new QuizSessionDto
        {
            AttemptId = attempt.Id,
            LanguageId = lang.Id,
            LanguageCode = lang.Code,
            LevelId = request.LevelId,
            LevelCode = level?.Code,
            ExamFormat = examFormat,
            ExamNumber = request.ExamNumber,
            DurationMinutes = isOfficialMock ? durationMinutes : null,
            TotalQuestions = questionDtos.Count,
            ListeningCount = questionDtos.Count(q => q.Section == "listening"),
            ReadingCount = questionDtos.Count(q => q.Section == "reading"),
            WritingCount = questionDtos.Count(q => q.Section == "writing" || q.QuestionType == "written"),
            McqCount = questionDtos.Count(q => q.QuestionType == "mcq"),
            WrittenCount = questionDtos.Count(q => q.QuestionType == "written"),
            StartedAt = attempt.StartedAt,
            NewWordsHarvestedCount = harvestBatch?.InsertedCount ?? 0,
            TotalWordsInBank = totalWordsInBank,
            HarvestSource = harvestBatch?.SourceLabel,
            NewlyHarvestedWords = harvestBatch?.InsertedWords ?? [],
            Questions = questionDtos
        };

        return Ok(session);
    }

    /// <summary>
    /// Nộp bài kiểm tra (chấm tự động cả Trắc nghiệm & Tự luận + AI chấm câu đặt) và lưu kết quả
    /// </summary>
    [HttpPost("{attemptId:int}/submit")]
    public async Task<ActionResult<QuizResultDto>> SubmitQuiz(
        int attemptId,
        [FromBody] SubmitQuizRequest request,
        CancellationToken ct)
    {
        var userId = GetRequiredUserId();

        var attempt = await dbContext.QuizAttempts
            .Include(a => a.Language)
            .Include(a => a.Level)
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy lượt kiểm tra ID {attemptId}.");

        if (attempt.CompletedAt.HasValue)
        {
            throw new BadHttpRequestException("Bài kiểm tra này đã được nộp và chấm điểm trước đó.");
        }

        var questionIds = request.Answers.Select(a => a.QuestionId).Distinct().ToList();
        var questions = await dbContext.Questions
            .Include(q => q.Options)
            .Where(q => questionIds.Contains(q.Id))
            .ToDictionaryAsync(q => q.Id, ct);

        // Nạp bảng ánh xạ từ vựng để tự động cập nhật tiến độ thuộc từ (UserWordProgress) khi làm bài kiểm tra
        var langWords = await dbContext.Words
            .AsNoTracking()
            .Where(w => w.LanguageId == attempt.LanguageId)
            .Select(w => new { w.Id, w.Term, w.Meaning })
            .ToListAsync(ct);

        var wordByTerm = langWords
            .Where(w => VocabHarvesterService.IsValidVocabularyWord(w.Term, w.Meaning))
            .GroupBy(w => w.Term.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

        var wordByMeaning = langWords
            .Where(w => VocabHarvesterService.IsValidVocabularyWord(w.Term, w.Meaning))
            .GroupBy(w => w.Meaning.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

        var userWordProgressMap = await dbContext.UserWordProgress
            .Where(p => p.UserId == userId && p.Word.LanguageId == attempt.LanguageId)
            .ToDictionaryAsync(p => p.WordId, ct);

        int score = 0;
        int listeningCorrect = 0;
        int listeningTotal = 0;
        int readingCorrect = 0;
        int readingTotal = 0;
        int mcqCorrect = 0;
        int mcqTotal = 0;
        int writtenCorrect = 0;
        int writtenTotal = 0;
        int aiGrammarCallsLeft = 2;
        var details = new List<QuizAnswerDetailDto>();
        var now = DateTime.UtcNow;

        foreach (var ans in request.Answers)
        {
            if (!questions.TryGetValue(ans.QuestionId, out var question))
            {
                continue;
            }

            var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
            var expectedText = (correctOption?.Text ?? string.Empty).Trim();

            if (TryParseWrittenQuestion(question.Text, out var subtype, out var cleanPrompt))
            {
                writtenTotal++;
                var userWritten = (ans.WrittenAnswer ?? string.Empty).Trim();
                bool isCorrect;
                string aiFeedback;

                if (subtype == "make_sentence")
                {
                    if (aiGrammarCallsLeft > 0)
                    {
                        (isCorrect, aiFeedback) = await GradeSentenceQuestionAsync(
                            userWritten,
                            expectedText,
                            attempt.Language.Code,
                            ct);
                        aiGrammarCallsLeft--;
                    }
                    else
                    {
                        bool hasKey = ContainsTargetKeyword(userWritten, expectedText, attempt.Language.Code);
                        bool hasLen = HasValidSentenceLength(userWritten, expectedText, attempt.Language.Code);
                        isCorrect = hasKey && hasLen;
                        aiFeedback = isCorrect
                            ? $"✅ Đạt yêu cầu! Bạn đã sử dụng đúng từ vựng \"{expectedText}\"."
                            : $"⚠️ Câu chưa đạt yêu cầu hoặc chưa chứa từ vựng bắt buộc \"{expectedText}\".";
                    }
                }
                else
                {
                    (isCorrect, aiFeedback) = GradeVocabWrittenAnswer(userWritten, expectedText);
                }

                if (isCorrect)
                {
                    score++;
                    writtenCorrect++;
                }

                dbContext.QuizAnswers.Add(new QuizAnswer
                {
                    QuizAttemptId = attempt.Id,
                    QuestionId = question.Id,
                    SelectedOptionId = null,
                    IsCorrect = isCorrect
                });

                TrackVocabularyMastery(expectedText, isCorrect, userId, now, wordByTerm, wordByMeaning, userWordProgressMap);

                details.Add(new QuizAnswerDetailDto
                {
                    QuestionId = question.Id,
                    QuestionText = FormatReviewQuestionTitle(subtype, cleanPrompt),
                    QuestionType = "written",
                    Section = "writing",
                    UserWrittenAnswer = string.IsNullOrWhiteSpace(userWritten) ? null : userWritten,
                    CorrectOptionId = correctOption?.Id ?? 0,
                    CorrectOptionText = subtype == "make_sentence"
                        ? $"Câu hoàn chỉnh chứa từ \"{expectedText}\""
                        : expectedText,
                    IsCorrect = isCorrect,
                    Explanation = question.Explanation,
                    AiFeedback = aiFeedback
                });
            }
            else
            {
                bool isListening = TryParseListeningQuestion(question.Text, out var lsSubtype, out var audioScript, out var cleanLsPrompt);
                string section = isListening ? "listening" : "reading";

                mcqTotal++;
                if (isListening) listeningTotal++;
                else readingTotal++;

                var selectedOption = ans.SelectedOptionId.HasValue
                    ? question.Options.FirstOrDefault(o => o.Id == ans.SelectedOptionId.Value)
                    : null;

                bool isCorrect = selectedOption != null && correctOption != null && selectedOption.Id == correctOption.Id;
                if (isCorrect)
                {
                    score++;
                    mcqCorrect++;
                    if (isListening) listeningCorrect++;
                    else readingCorrect++;
                }

                dbContext.QuizAnswers.Add(new QuizAnswer
                {
                    QuizAttemptId = attempt.Id,
                    QuestionId = question.Id,
                    SelectedOptionId = selectedOption?.Id,
                    IsCorrect = isCorrect
                });

                TrackVocabularyMastery(expectedText, isCorrect, userId, now, wordByTerm, wordByMeaning, userWordProgressMap);

                details.Add(new QuizAnswerDetailDto
                {
                    QuestionId = question.Id,
                    QuestionText = isListening ? cleanLsPrompt : question.Text,
                    QuestionType = "mcq",
                    Section = section,
                    SelectedOptionId = selectedOption?.Id,
                    SelectedOptionText = selectedOption?.Text,
                    CorrectOptionId = correctOption?.Id ?? 0,
                    CorrectOptionText = expectedText,
                    IsCorrect = isCorrect,
                    Explanation = question.Explanation
                });
            }
        }

        attempt.Score = Math.Clamp(score, 0, attempt.TotalQuestions);
        attempt.CompletedAt = now;

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new KeyNotFoundException("Không tìm thấy người dùng.");

        double pct = attempt.TotalQuestions > 0 ? (double)attempt.Score / attempt.TotalQuestions * 100 : 0;
        int bonusXp = pct >= 100 ? 30 : pct >= 70 ? 15 : 5;
        int rawEarnedXp = (mcqCorrect * 10) + (writtenCorrect * 15) + bonusXp;
        var xpAward = GamificationHelper.AwardXp(user, rawEarnedXp);

        await dbContext.SaveChangesAsync(ct);

        return Ok(new QuizResultDto
        {
            AttemptId = attempt.Id,
            LanguageId = attempt.LanguageId,
            LanguageCode = attempt.Language.Code,
            LevelId = attempt.LevelId,
            LevelCode = attempt.Level?.Code,
            Score = attempt.Score,
            TotalQuestions = attempt.TotalQuestions,
            ListeningCorrect = listeningCorrect,
            ListeningTotal = listeningTotal,
            ReadingCorrect = readingCorrect,
            ReadingTotal = readingTotal,
            McqCorrect = mcqCorrect,
            McqTotal = mcqTotal,
            WrittenCorrect = writtenCorrect,
            WrittenTotal = writtenTotal,
            StartedAt = attempt.StartedAt,
            CompletedAt = attempt.CompletedAt,
            XpEarned = xpAward.XpEarned,
            TotalXp = xpAward.TotalXp,
            TodayXp = xpAward.TodayXp,
            DailyGoalXp = xpAward.DailyGoalXp,
            Level = xpAward.Level,
            LeveledUp = xpAward.LeveledUp,
            Details = details
        });
    }

    /// <summary>
    /// Lấy lịch sử các lần làm bài kiểm tra của người dùng
    /// </summary>
    [HttpGet("history")]
    public async Task<ActionResult<PagedResult<QuizHistoryDto>>> GetHistory(
        [FromQuery] PaginationParams pagination,
        CancellationToken ct)
    {
        var userId = GetRequiredUserId();

        var query = dbContext.QuizAttempts
            .AsNoTracking()
            .Include(a => a.Language)
            .Include(a => a.Level)
            .Where(a => a.UserId == userId && a.CompletedAt.HasValue)
            .OrderByDescending(a => a.CompletedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(a => new QuizHistoryDto
            {
                AttemptId = a.Id,
                LanguageId = a.LanguageId,
                LanguageCode = a.Language.Code,
                LevelId = a.LevelId,
                LevelCode = a.Level != null ? a.Level.Code : null,
                Score = a.Score,
                TotalQuestions = a.TotalQuestions,
                StartedAt = a.StartedAt,
                CompletedAt = a.CompletedAt
            })
            .ToListAsync(ct);

        return Ok(new PagedResult<QuizHistoryDto>(items, totalCount, pagination.PageNumber, pagination.PageSize));
    }

    /// <summary>
    /// Xem lại chi tiết kết quả một bài kiểm tra đã nộp
    /// </summary>
    [HttpGet("{attemptId:int}")]
    public async Task<ActionResult<QuizResultDto>> GetResult(int attemptId, CancellationToken ct)
    {
        var userId = GetRequiredUserId();

        var attempt = await dbContext.QuizAttempts
            .AsNoTracking()
            .Include(a => a.Language)
            .Include(a => a.Level)
            .Include(a => a.Answers)
                .ThenInclude(ans => ans.Question)
                    .ThenInclude(q => q.Options)
            .Include(a => a.Answers)
                .ThenInclude(ans => ans.SelectedOption)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId, ct)
            ?? throw new KeyNotFoundException($"Không tìm thấy bài kiểm tra ID {attemptId}.");

        int mcqCorrect = 0, mcqTotal = 0, writtenCorrect = 0, writtenTotal = 0;
        int listeningCorrect = 0, listeningTotal = 0, readingCorrect = 0, readingTotal = 0;

        var details = attempt.Answers.Select(ans =>
        {
            var correctOption = ans.Question.Options.FirstOrDefault(o => o.IsCorrect);
            var expected = correctOption?.Text ?? string.Empty;

            if (TryParseWrittenQuestion(ans.Question.Text, out var subtype, out var cleanPrompt))
            {
                writtenTotal++;
                if (ans.IsCorrect) writtenCorrect++;
                return new QuizAnswerDetailDto
                {
                    QuestionId = ans.QuestionId,
                    QuestionText = FormatReviewQuestionTitle(subtype, cleanPrompt),
                    QuestionType = "written",
                    Section = "writing",
                    CorrectOptionId = correctOption?.Id ?? 0,
                    CorrectOptionText = subtype == "make_sentence"
                        ? $"Câu hoàn chỉnh chứa từ \"{expected}\""
                        : expected,
                    IsCorrect = ans.IsCorrect,
                    Explanation = ans.Question.Explanation
                };
            }

            if (TryParseListeningQuestion(ans.Question.Text, out _, out _, out var cleanLsPrompt))
            {
                listeningTotal++;
                mcqTotal++;
                if (ans.IsCorrect)
                {
                    listeningCorrect++;
                    mcqCorrect++;
                }
                return new QuizAnswerDetailDto
                {
                    QuestionId = ans.QuestionId,
                    QuestionText = cleanLsPrompt,
                    QuestionType = "mcq",
                    Section = "listening",
                    SelectedOptionId = ans.SelectedOptionId,
                    SelectedOptionText = ans.SelectedOption?.Text,
                    CorrectOptionId = correctOption?.Id ?? 0,
                    CorrectOptionText = expected,
                    IsCorrect = ans.IsCorrect,
                    Explanation = ans.Question.Explanation
                };
            }

            readingTotal++;
            mcqTotal++;
            if (ans.IsCorrect)
            {
                readingCorrect++;
                mcqCorrect++;
            }
            return new QuizAnswerDetailDto
            {
                QuestionId = ans.QuestionId,
                QuestionText = ans.Question.Text,
                QuestionType = "mcq",
                Section = "reading",
                SelectedOptionId = ans.SelectedOptionId,
                SelectedOptionText = ans.SelectedOption?.Text,
                CorrectOptionId = correctOption?.Id ?? 0,
                CorrectOptionText = expected,
                IsCorrect = ans.IsCorrect,
                Explanation = ans.Question.Explanation
            };
        }).ToList();

        return Ok(new QuizResultDto
        {
            AttemptId = attempt.Id,
            LanguageId = attempt.LanguageId,
            LanguageCode = attempt.Language.Code,
            LevelId = attempt.LevelId,
            LevelCode = attempt.Level?.Code,
            Score = attempt.Score,
            TotalQuestions = attempt.TotalQuestions,
            ListeningCorrect = listeningCorrect,
            ListeningTotal = listeningTotal,
            ReadingCorrect = readingCorrect,
            ReadingTotal = readingTotal,
            McqCorrect = mcqCorrect,
            McqTotal = mcqTotal,
            WrittenCorrect = writtenCorrect,
            WrittenTotal = writtenTotal,
            StartedAt = attempt.StartedAt,
            CompletedAt = attempt.CompletedAt,
            Details = details
        });
    }

    private void TrackVocabularyMastery(
        string expectedText,
        bool isCorrect,
        int userId,
        DateTime now,
        Dictionary<string, int> wordByTerm,
        Dictionary<string, int> wordByMeaning,
        Dictionary<int, UserWordProgress> progressMap)
    {
        if (string.IsNullOrWhiteSpace(expectedText)) return;

        if (!wordByTerm.TryGetValue(expectedText, out var wordId) &&
            !wordByMeaning.TryGetValue(expectedText, out wordId))
        {
            return;
        }

        if (!progressMap.TryGetValue(wordId, out var progress))
        {
            var initialBox = isCorrect ? 1 : 0;
            progress = new UserWordProgress
            {
                UserId = userId,
                WordId = wordId,
                Box = initialBox,
                Status = WordStatus.Learning,
                ReviewCount = 1,
                LastReviewedAt = now,
                NextReviewAt = now.AddDays(initialBox == 1 ? 2 : 1)
            };
            dbContext.UserWordProgress.Add(progress);
            progressMap[wordId] = progress;
        }
        else
        {
            progress.Box = isCorrect ? Math.Min(5, progress.Box + 1) : Math.Max(0, progress.Box - 1);
            progress.ReviewCount += 1;
            progress.LastReviewedAt = now;
            progress.Status = progress.Box >= 5 ? WordStatus.Mastered : WordStatus.Learning;
            progress.NextReviewAt = now.AddDays(progress.Box switch
            {
                0 => 1,
                1 => 2,
                2 => 4,
                3 => 7,
                4 => 14,
                _ => 30
            });
        }
    }

    private static string NormalizeExamFormat(string? raw)
    {
        var clean = (raw ?? "combined").Trim().ToLowerInvariant();
        return clean is "mcq" or "written" or "combined" or "official_mock" ? clean : "combined";
    }

    private static bool TryParseListeningQuestion(string rawText, out string subtype, out string audioScript, out string cleanPrompt)
    {
        subtype = string.Empty;
        audioScript = string.Empty;
        cleanPrompt = rawText;

        if (string.IsNullOrWhiteSpace(rawText) || !rawText.StartsWith("[LS:", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var closeBracket = rawText.IndexOf(']');
        if (closeBracket <= 4)
        {
            return false;
        }

        var header = rawText[4..closeBracket];
        var splitIdx = header.IndexOf('|');
        if (splitIdx > 0)
        {
            subtype = header[..splitIdx].Trim().ToLowerInvariant();
            audioScript = header[(splitIdx + 1)..].Trim();
        }
        else
        {
            subtype = header.Trim().ToLowerInvariant();
            audioScript = string.Empty;
        }

        cleanPrompt = rawText[(closeBracket + 1)..].Trim();
        return true;
    }

    private static bool TryParseWrittenQuestion(string rawText, out string subtype, out string cleanPrompt)
    {
        subtype = string.Empty;
        cleanPrompt = rawText;

        if (rawText.StartsWith("[TL:TERM]", StringComparison.OrdinalIgnoreCase))
        {
            subtype = "write_term";
            cleanPrompt = rawText[9..].Trim();
            return true;
        }
        if (rawText.StartsWith("[TL:CLOZE]", StringComparison.OrdinalIgnoreCase))
        {
            subtype = "cloze";
            cleanPrompt = rawText[10..].Trim();
            return true;
        }
        if (rawText.StartsWith("[TL:SENTENCE]", StringComparison.OrdinalIgnoreCase))
        {
            subtype = "make_sentence";
            cleanPrompt = rawText[13..].Trim();
            return true;
        }

        return false;
    }

    private static string FormatReviewQuestionTitle(string subtype, string cleanPrompt)
    {
        var label = subtype switch
        {
            "make_sentence" => "[Tự luận • Đặt câu]",
            "cloze" => "[Tự luận • Điền từ]",
            _ => "[Tự luận • Viết từ]"
        };
        return $"{label} {cleanPrompt}";
    }

    private static string BuildWrittenHint(string subtype, string expectedTerm)
    {
        if (string.IsNullOrWhiteSpace(expectedTerm))
        {
            return "Nhập câu trả lời bằng ngôn ngữ đang học.";
        }

        if (subtype == "make_sentence")
        {
            return $"Viết tối thiểu 1 câu hoàn chỉnh có chứa từ \"{expectedTerm}\" (Trợ lý AI sẽ chấm ngữ pháp & gợi ý diễn đạt).";
        }

        var trimmed = expectedTerm.Trim();
        if (trimmed.Length == 1)
        {
            return "Từ vựng này gồm 1 chữ/ký tự duy nhất.";
        }

        var firstChar = trimmed[0];
        var lastChar = trimmed[^1];
        if (trimmed.Length >= 5)
        {
            return $"Bắt đầu bằng chữ \"{firstChar}...\", kết thúc bằng \"...{lastChar}\" • Tổng cộng {trimmed.Length} ký tự.";
        }

        return $"Bắt đầu bằng \"{firstChar}...\" • Độ dài {trimmed.Length} ký tự.";
    }

    private static (bool IsCorrect, string Feedback) GradeVocabWrittenAnswer(string userRaw, string expectedRaw)
    {
        if (string.IsNullOrWhiteSpace(userRaw))
        {
            return (false, "⚠️ Bạn chưa điền đáp án cho câu tự luận này.");
        }

        var normUser = NormalizeVocabString(userRaw);
        var candidates = expectedRaw
            .Split(['/', ';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Append(expectedRaw)
            .Select(NormalizeVocabString)
            .Where(s => s.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var cand in candidates)
        {
            if (string.Equals(normUser, cand, StringComparison.OrdinalIgnoreCase))
            {
                return (true, "✅ Chính xác tuyệt đối! Bạn nhớ từ và chính tả rất tốt.");
            }

            // Chấp nhận linh hoạt tiền tố "to " đối với động từ tiếng Anh
            var userNoTo = normUser.StartsWith("to ", StringComparison.OrdinalIgnoreCase) ? normUser[3..].Trim() : normUser;
            var candNoTo = cand.StartsWith("to ", StringComparison.OrdinalIgnoreCase) ? cand[3..].Trim() : cand;
            if (userNoTo.Length > 0 && string.Equals(userNoTo, candNoTo, StringComparison.OrdinalIgnoreCase))
            {
                return (true, "✅ Chính xác! Bạn đã ghi nhớ đúng từ vựng mục tiêu.");
            }
        }

        var primaryExpected = NormalizeVocabString(expectedRaw);
        if (primaryExpected.Length >= 4 && ComputeLevenshteinDistance(normUser, primaryExpected) == 1)
        {
            return (false, $"💡 Rất tiếc! Bạn viết gần đúng rồi, chỉ lệch 1 ký tự so với đáp án chuẩn \"{expectedRaw}\".");
        }

        return (false, $"💡 Chưa chính xác. Đáp án đúng cần viết là \"{expectedRaw}\".");
    }

    private async Task<(bool IsCorrect, string Feedback)> GradeSentenceQuestionAsync(
        string userSentence,
        string expectedKeyword,
        string languageCode,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(userSentence))
        {
            return (false, "⚠️ Bạn chưa viết câu cho phần tự luận đặt câu này.");
        }

        if (!ContainsTargetKeyword(userSentence, expectedKeyword, languageCode))
        {
            return (false, $"⚠️ Câu của bạn chưa sử dụng từ vựng bắt buộc \"{expectedKeyword}\". Hãy đưa từ này vào câu nhé!");
        }

        if (!HasValidSentenceLength(userSentence, expectedKeyword, languageCode))
        {
            return (false, $"⚠️ Câu còn quá ngắn (chưa tạo thành câu hoàn chỉnh). Hãy viết tối thiểu một câu đầy đủ ý với từ \"{expectedKeyword}\".");
        }

        try
        {
            var grammarRes = await aiService.CheckGrammarAsync(new GrammarCheckRequest
            {
                Text = userSentence,
                LanguageCode = languageCode,
                TargetMeaning = expectedKeyword
            }, ct);

            if (grammarRes.IsCorrect)
            {
                var exp = !string.IsNullOrWhiteSpace(grammarRes.Explanation)
                    ? grammarRes.Explanation
                    : "Câu đặt tự nhiên và đúng ngữ pháp!";
                return (true, $"🤖 AI Chấm câu (Đạt): {exp}");
            }

            var correctionNote = !string.IsNullOrWhiteSpace(grammarRes.CorrectedText)
                ? $" • Gợi ý sửa chuẩn: \"{grammarRes.CorrectedText}\""
                : string.Empty;
            return (false, $"🤖 AI Chấm & Sửa ngữ pháp: {grammarRes.Explanation}{correctionNote}");
        }
        catch
        {
            return (true, $"✅ Đạt yêu cầu! Bạn đã đặt câu hoàn chỉnh có sử dụng đúng từ \"{expectedKeyword}\".");
        }
    }

    private static bool ContainsTargetKeyword(string sentence, string keyword, string langCode)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return true;
        var cleanKey = keyword.Trim();
        if (sentence.Contains(cleanKey, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Hỗ trợ dạng chia động từ / số nhiều trong tiếng Anh (VD: Negotiate -> negotiated / negotiating)
        if (string.Equals(langCode, "en", StringComparison.OrdinalIgnoreCase) && cleanKey.Length >= 5)
        {
            var stem = cleanKey[..^1];
            if (sentence.Contains(stem, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasValidSentenceLength(string sentence, string keyword, string langCode)
    {
        var trimmed = sentence.Trim();
        var isCjk = langCode is "zh" or "ja" or "ko";
        if (isCjk)
        {
            return trimmed.Length >= Math.Max(4, keyword.Trim().Length + 2);
        }

        var words = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Length >= 3 && trimmed.Length >= keyword.Trim().Length + 4;
    }

    private static string NormalizeVocabString(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        var withoutParens = Regex.Replace(input, @"\([^)]*\)", " ");
        var trimmedPunct = withoutParens.Trim().Trim('"', '\'', '.', ',', '!', '?', '。', '！', '？', ' ', '\t');
        return Regex.Replace(trimmedPunct, @"\s+", " ").ToLowerInvariant();
    }

    private static int ComputeLevenshteinDistance(string a, string b)
    {
        if (a == b) return 0;
        if (a.Length == 0) return b.Length;
        if (b.Length == 0) return a.Length;

        var dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                var cost = char.ToLowerInvariant(a[i - 1]) == char.ToLowerInvariant(b[j - 1]) ? 0 : 1;
                dp[i, j] = Math.Min(
                    Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                    dp[i - 1, j - 1] + cost);
            }
        }
        return dp[a.Length, b.Length];
    }

    private int GetRequiredUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Cần đăng nhập để sử dụng tính năng này.");
        }
        return userId;
    }
}
