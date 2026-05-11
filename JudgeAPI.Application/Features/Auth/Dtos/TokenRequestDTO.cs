
namespace JudgeAPI.Application.Features.Auth.Dtos
{
    public class TokenRequestDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
