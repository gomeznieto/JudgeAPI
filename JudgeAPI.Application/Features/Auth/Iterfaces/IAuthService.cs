using JudgeAPI.Application.Features.Auth.Dtos;
using JudgeAPI.Application.Features.Users.Dtos;

namespace JudgeAPI.Application.Features.Auth.Iterfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<TokenResponseDTO> RegisterAsync(UserCreateDTO dto);
        Task<TokenResponseDTO> RefreshTokenAsync(TokenRequestDTO dto);
    }
}
