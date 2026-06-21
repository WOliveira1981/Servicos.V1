using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ServicoApp.Application.Auth;

/// <summary>
/// Serviço responsável por gerar JWT para sessões autenticadas.
/// </summary>
public sealed class JwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(string email, string provider)
    {
        var secret = _configuration["Jwt:Key"] ?? "super-secret-key-for-dev-only-change-me";
        var issuer = _configuration["Jwt:Issuer"] ?? "ServicoApp";
        var audience = _configuration["Jwt:Audience"] ?? "ServicoAppClients";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.AuthenticationMethod, provider),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation("JWT gerado para {Email}", email);
        return jwt;
    }
}