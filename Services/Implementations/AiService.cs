using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PolyLearnAI.Api.Models.DTOs.Ai;
using PolyLearnAI.Api.Options;
using PolyLearnAI.Api.Services.Interfaces;

namespace PolyLearnAI.Api.Services.Implementations;

public class AiService(
    HttpClient httpClient,
    IOptions<AiOptions> options,
    ILogger<AiService> logger) : IAiService
{
    private readonly AiOptions _options = options.Value;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public bool HasApiKey => !string.IsNullOrWhiteSpace(_options.ApiKey);

    public async Task<GrammarCheckResponse> CheckGrammarAsync(GrammarCheckRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return MockGrammarCheck(request);
        }

        try
        {
            var prompt = $$"""
Bạn là chuyên gia ngôn ngữ và giáo viên dạy học ngoại ngữ.
Hãy kiểm tra ngữ pháp cho câu sau đây của học viên:
Ngôn ngữ: {{request.LanguageCode}}
Câu của học viên: "{{request.Text}}"
Ý nghĩa mong muốn: "{{request.TargetMeaning ?? "Không có"}}"

Hãy trả về DUY NHẤT một chuỗi JSON hợp lệ (không kèm markdown ```json hay văn bản giải thích nào khác) theo cấu trúc sau:
{
  "originalText": "{{request.Text}}",
  "isCorrect": boolean,
  "correctedText": "câu đã được sửa cho đúng và tự nhiên nhất",
  "explanation": "giải thích chi tiết bằng tiếng Việt về lỗi sai và cách dùng đúng",
  "suggestions": ["cách diễn đạt tự nhiên 1", "cách diễn đạt tự nhiên 2"],
  "grammarPoints": ["điểm ngữ pháp liên quan 1", "điểm ngữ pháp liên quan 2"]
}
""";

            var content = await CallGeminiAsync(prompt, ct);
            if (!string.IsNullOrWhiteSpace(content))
            {
                var cleaned = CleanJsonString(content);
                var result = JsonSerializer.Deserialize<GrammarCheckResponse>(cleaned, JsonOpts);
                if (result != null) return result;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Lỗi khi gọi AI API CheckGrammar, chuyển sang chế độ dự phòng.");
        }

        return MockGrammarCheck(request);
    }

    public async Task<WordExplanationResponse> ExplainWordAsync(WordExplanationRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return MockExplainWord(request);
        }

        try
        {
            var prompt = $$"""
Bạn là từ điển và trợ lý ngôn ngữ thông minh.
Hãy giải thích từ/cụm từ sau đây cho học viên người Việt:
Từ: "{{request.Term}}"
Ngôn ngữ: {{request.LanguageCode}}
Ngữ cảnh: "{{request.ContextSentence ?? "Không có"}}"

Hãy trả về DUY NHẤT một chuỗi JSON hợp lệ (không kèm markdown ```json hay văn bản giải thích nào khác) theo cấu trúc sau:
{
  "term": "{{request.Term}}",
  "reading": "phiên âm quốc tế IPA / pinyin / romaji",
  "meaning": "nghĩa ngắn gọn súc tích",
  "detailedExplanation": "giải thích chi tiết ngữ cảnh sử dụng, sắc thái từ bằng tiếng Việt",
  "sampleSentences": [
    {
      "sentence": "câu ví dụ 1",
      "reading": "phiên âm câu 1",
      "translation": "dịch nghĩa tiếng Việt câu 1"
    },
    {
      "sentence": "câu ví dụ 2",
      "reading": "phiên âm câu 2",
      "translation": "dịch nghĩa tiếng Việt câu 2"
    }
  ],
  "commonCollocations": ["cụm từ hay đi kèm 1", "cụm từ hay đi kèm 2"]
}
""";

            var content = await CallGeminiAsync(prompt, ct);
            if (!string.IsNullOrWhiteSpace(content))
            {
                var cleaned = CleanJsonString(content);
                var result = JsonSerializer.Deserialize<WordExplanationResponse>(cleaned, JsonOpts);
                if (result != null) return result;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Lỗi khi gọi AI API ExplainWord, chuyển sang chế độ dự phòng.");
        }

        return MockExplainWord(request);
    }

    public async Task<AiChatResponse> ChatAsync(AiChatRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            return MockChat(request);
        }

        try
        {
            var historyText = string.Join("\n", request.Messages.Select(m => $"{m.Role}: {m.Content}"));
            var prompt = $$"""
Bạn là đối tác luyện giao tiếp ngoại ngữ thân thiện, kiên nhẫn.
Mục tiêu: Đóng vai trò phù hợp trong tình huống và trò chuyện bằng ngôn ngữ: {{request.LanguageCode}} (Cấp độ: {{request.LevelCode ?? "A1"}}).
Tình huống: {{request.Scenario}}.
Lịch sử cuộc trò chuyện:
{{historyText}}

Hãy phản hồi tin nhắn mới nhất của học viên và gợi ý các câu trả lời tiếp theo.
Trả về DUY NHẤT chuỗi JSON (không kèm markdown ```json):
{
  "reply": "câu phản hồi của AI bằng ngôn ngữ đang học",
  "reading": "phiên âm pinyin/romaji nếu là tiếng Trung/Nhật/Hàn",
  "vietnameseTranslation": "dịch nghĩa tiếng Việt của câu reply",
  "suggestedReplies": ["câu gợi ý trả lời 1", "câu gợi ý trả lời 2"]
}
""";

            var content = await CallGeminiAsync(prompt, ct);
            if (!string.IsNullOrWhiteSpace(content))
            {
                var cleaned = CleanJsonString(content);
                var result = JsonSerializer.Deserialize<AiChatResponse>(cleaned, JsonOpts);
                if (result != null) return result;
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Lỗi khi gọi AI API Chat, chuyển sang chế độ dự phòng.");
        }

        return MockChat(request);
    }

    public async Task<IReadOnlyList<GeneratedVocabItemDto>> GenerateVocabularyBatchAsync(
        string languageCode,
        string? topicHint,
        IReadOnlyCollection<string> excludeTerms,
        int count = 6,
        CancellationToken ct = default)
    {
        if (!HasApiKey)
        {
            return [];
        }

        try
        {
            var langLabel = languageCode switch
            {
                "zh" => "Tiếng Trung (chữ Hán giản thể, kèm Pinyin có thanh điệu)",
                "ja" => "Tiếng Nhật (Kanji/Kana tự nhiên, kèm cách đọc Hiragana/Romaji)",
                "ko" => "Tiếng Hàn (Hangul, kèm phiên âm Romaja)",
                _ => "Tiếng Anh (kèm phiên âm quốc tế IPA)"
            };
            var sampleExclude = string.Join(", ", excludeTerms.Take(80));
            var topicDesc = string.IsNullOrWhiteSpace(topicHint) || topicHint == "random"
                ? "đa dạng thực tế đời sống, công nghệ, du lịch, giao tiếp, công việc"
                : topicHint;

            var prompt = $$"""
Bạn là chuyên gia biên soạn từ điển ngoại ngữ chuẩn quốc tế cho người Việt.
Hãy tạo mới {{count}} từ vựng thuộc ngôn ngữ: {{langLabel}}.
Chủ đề ưu tiên: "{{topicDesc}}".
Tuyệt đối KHÔNG trùng với các từ đã có sau: [{{sampleExclude}}].

Trả về DUY NHẤT một mảng JSON hợp lệ (không kèm markdown ```json):
[
  {
    "term": "từ vựng chuẩn bản xứ",
    "reading": "phiên âm IPA / Pinyin / Hiragana / Romaja",
    "meaning": "nghĩa tiếng Việt ngắn gọn, chính xác",
    "partOfSpeech": "noun | verb | adjective | adverb | phrase",
    "exampleSentence": "1 câu ví dụ tự nhiên chứa từ đó",
    "exampleReading": "phiên âm câu ví dụ (nếu là zh/ja/ko, để null nếu là en)",
    "exampleTranslation": "dịch câu ví dụ sang tiếng Việt"
  }
]
""";

            var content = await CallGeminiAsync(prompt, ct);
            if (!string.IsNullOrWhiteSpace(content))
            {
                var cleaned = CleanJsonString(content);
                var items = JsonSerializer.Deserialize<List<GeneratedVocabItemDto>>(cleaned, JsonOpts);
                if (items is { Count: > 0 })
                {
                    return items
                        .Where(x => !string.IsNullOrWhiteSpace(x.Term) && !string.IsNullOrWhiteSpace(x.Meaning))
                        .ToList();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Lỗi khi gọi Gemini sinh bộ từ vựng mới, hệ thống sẽ chuyển sang nguồn từ điển mở rộng.");
        }

        return [];
    }

    private async Task<string?> CallGeminiAsync(string prompt, CancellationToken ct)
    {
        var model = string.IsNullOrWhiteSpace(_options.Model) ? "gemini-1.5-flash" : _options.Model;
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_options.ApiKey}";

        var payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.4,
                responseMimeType = "application/json"
            }
        };

        var response = await httpClient.PostAsJsonAsync(url, payload, ct);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            logger.LogError("Gemini API call thất bại với mã {StatusCode}: {Error}", response.StatusCode, error);
            return null;
        }

        var jsonDoc = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken: ct);
        var text = jsonDoc?.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return text;
    }

    private static string CleanJsonString(string raw)
    {
        var str = raw.Trim();
        if (str.StartsWith("```json")) str = str[7..];
        if (str.StartsWith("```")) str = str[3..];
        if (str.EndsWith("```")) str = str[..^3];
        return str.Trim();
    }

    private static GrammarCheckResponse MockGrammarCheck(GrammarCheckRequest req)
    {
        return new GrammarCheckResponse
        {
            OriginalText = req.Text,
            IsCorrect = true,
            CorrectedText = req.Text.Trim(),
            Explanation = $"Câu \"{req.Text}\" của bạn đã được diễn đạt rõ ý. Để nghe tự nhiên hơn như người bản xứ, bạn có thể tham khảo thêm các gợi ý bên dưới.",
            Suggestions =
            [
                $"{req.Text} (Cách dùng tiêu chuẩn)",
                $"I would say: {req.Text}."
            ],
            GrammarPoints =
            [
                "Cấu trúc câu hoàn chỉnh (Chủ ngữ + Động từ + Tân ngữ)",
                "Đúng ngữ cảnh giao tiếp"
            ]
        };
    }

    private static WordExplanationResponse MockExplainWord(WordExplanationRequest req)
    {
        return new WordExplanationResponse
        {
            Term = req.Term,
            Reading = req.LanguageCode switch
            {
                "zh" => "pīnyīn",
                "ja" => "romaji",
                "ko" => "romanization",
                _ => "/ˈtɜːm/"
            },
            Meaning = $"Nghĩa thông dụng của \"{req.Term}\"",
            DetailedExplanation = $"Từ \"{req.Term}\" thường được sử dụng trong các ngữ cảnh giao tiếp hằng ngày và học thuật cơ bản. Phù hợp cho người học từ trình độ sơ cấp đến trung cấp.",
            SampleSentences =
            [
                new()
                {
                    Sentence = $"This is an example with {req.Term}.",
                    Reading = null,
                    Translation = $"Đây là một câu ví dụ với {req.Term}."
                }
            ],
            CommonCollocations =
            [
                $"use {req.Term}",
                $"learn {req.Term}"
            ]
        };
    }

    private static AiChatResponse MockChat(AiChatRequest req)
    {
        var lastMsg = req.Messages.LastOrDefault()?.Content ?? "Hello";
        return new AiChatResponse
        {
            Reply = req.LanguageCode switch
            {
                "zh" => "你好！很高兴和你聊天。你今天过得怎么样？",
                "ja" => "こんにちは！お話しできて嬉しいです。今日はどんな一日でしたか？",
                "ko" => "안녕하세요! 대화하게 되어 반갑습니다. 오늘 하루 어떠셨어요?",
                _ => $"Hello! Nice to chat with you. You said: \"{lastMsg}\". How are you doing today?"
            },
            Reading = req.LanguageCode switch
            {
                "zh" => "Nǐ hǎo! Hěn gāoxìng hé nǐ liáotiān. Nǐ jīntiān guò de zěnmeyàng?",
                "ja" => "Konnichiwa! Ohanashi dekite ureshii desu. Kyō wa donna ichinichi deshita ka?",
                "ko" => "Annyeonghaseyo! Daehwahage doeeo bangapseumnida. Oneul haru eotteosyeosseoyo?",
                _ => null
            },
            VietnameseTranslation = "Xin chào! Rất vui được trò chuyện với bạn. Bạn cảm thấy hôm nay thế nào?",
            SuggestedReplies =
            [
                req.LanguageCode switch
                {
                    "zh" => "我很好，谢谢！你呢？",
                    "ja" => "元気です。あなたは？",
                    "ko" => "잘 지내고 있어요. 당신은요?",
                    _ => "I'm doing great, thank you! How about you?"
                },
                req.LanguageCode switch
                {
                    "zh" => "我今天有点忙。",
                    "ja" => "今日は少し忙しいです。",
                    "ko" => "오늘은 조금 바빠요.",
                    _ => "I'm a bit busy today."
                }
            ]
        };
    }
}
