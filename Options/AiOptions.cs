namespace PolyLearnAI.Api.Options;

public class AiOptions
{
    public const string SectionName = "Ai";

    public string Provider { get; set; } = "Gemini"; // "Gemini" | "OpenAI"
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
    public string? Endpoint { get; set; }
    public bool MockWhenNoApiKey { get; set; } = true;
}
