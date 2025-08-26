namespace JudgeAPI.Configuration
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;
        public string Issuer { get; set; } = "JudgeApi";
        public string Audience { get; set; } = "JudgeApiAudience";
    }
}
