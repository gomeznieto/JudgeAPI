namespace JudgeAPI.Application.Features;

public interface IAuthService {
    Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request);
    Task<TokenResponseDTO> RegisterAsync(UserCreateDTO dto);
}

