using PolyLearnAI.Api.Models.DTOs.Word;
using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Services.Interfaces;

public record HarvestBatchResult(
    int InsertedCount,
    int TotalWordsInLanguage,
    string SourceLabel,
    IReadOnlyList<WordDto> InsertedWords,
    IReadOnlyList<int> InsertedWordIds
);

public interface IVocabHarvesterService
{
    Task<HarvestBatchResult> HarvestAndPersistWordsAsync(
        int languageId,
        int count = 8,
        string? topicHint = null,
        int? levelId = null,
        CancellationToken ct = default);

    Task<IReadOnlyList<Question>> BuildDynamicListeningQuestionsAsync(
        int languageId,
        int? levelId,
        int questionCount,
        IReadOnlyList<int>? priorityWordIds = null,
        int examSeed = 1,
        CancellationToken ct = default);

    Task<IReadOnlyList<Question>> BuildDynamicVocabQuizQuestionsAsync(
        int languageId,
        int? levelId,
        int questionCount,
        IReadOnlyList<int>? priorityWordIds = null,
        int examSeed = 1,
        CancellationToken ct = default);

    Task<IReadOnlyList<Question>> BuildDynamicWrittenQuestionsAsync(
        int languageId,
        int? levelId,
        int questionCount,
        IReadOnlyList<int>? priorityWordIds = null,
        IReadOnlyCollection<string>? excludeTargetTerms = null,
        int examSeed = 1,
        CancellationToken ct = default);
}

