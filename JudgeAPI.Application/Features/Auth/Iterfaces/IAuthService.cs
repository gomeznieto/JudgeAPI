using JudgeAPI.Application.Features.Auth.Dtos;


namespace JudgeAPI.Application.Features.Auth.Iterfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request);
        Task<TokenResponseDTO> RegisterAsync(UserCreateDTO dto);
    }
}
