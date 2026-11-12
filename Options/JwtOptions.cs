namespace PolyLearnAI.Api.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = "PolyLearnAI";
    public string Audience { get; set; } = "PolyLearnAI.Client";
    /// <summary>Khóa ký JWT (>= 32 ký tự). Đặt qua User Secrets / biến môi trường, KHÔNG commit.</summary>
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 120;
}
