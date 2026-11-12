using PolyLearnAI.Api.Models.DTOs.Ai;

namespace PolyLearnAI.Api.Services.Interfaces;

public interface IAiService
{
    bool HasApiKey { get; }
    Task<GrammarCheckResponse> CheckGrammarAsync(GrammarCheckRequest request, CancellationToken ct = default);
    Task<WordExplanationResponse> ExplainWordAsync(WordExplanationRequest request, CancellationToken ct = default);
    Task<AiChatResponse> ChatAsync(AiChatRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<GeneratedVocabItemDto>> GenerateVocabularyBatchAsync(
        string languageCode,
        string? topicHint,
        IReadOnlyCollection<string> excludeTerms,
        int count = 6,
        CancellationToken ct = default);
}
