using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JudgeAPI.Application.Features.Auth.Iterfaces;
using System.Security.Cryptography;


namespace JudgeAPI.Infrastructure.Authentication;

public class TokenService(IOptions<JwtSettings> jwtSettings) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public string GenerateToken(string userId, string userName, IList<string> roles)
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, userId),
            new (JwtRegisteredClaimNames.Nickname, userName ?? ""),
            new (ClaimTypes.NameIdentifier, userId),
        };

        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        JwtSecurityToken token = new(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: creds
                );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string GetHashToken(string token)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            ValidateLifetime = false
        };

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Token inválido o algoritmo incorrecto");
        }

        return principal;
    }
}

