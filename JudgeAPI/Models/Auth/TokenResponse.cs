using JudgeAPI.Models.User;

namespace JudgeAPI.Models.Auth
{
    public class TokenResponse : UserPrivateDTO
    {
        public string Token { get; set; } = String.Empty;
        public string UserId { get; set; } = String.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
