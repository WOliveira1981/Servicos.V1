using Microsoft.Extensions.Logging;

namespace ServicoApp.Application.Auth;

/// <summary>
/// Estratégia concreta para autenticação via Google OAuth2.
/// </summary>
public sealed class GoogleOAuthStrategy : IAuthenticationStrategy
{
    private readonly ILogger<GoogleOAuthStrategy> _logger;

    public GoogleOAuthStrategy(ILogger<GoogleOAuthStrategy> logger)
    {
        _logger = logger;
    }

    public Task<AuthResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(request.Provider, "google", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new AuthResult(false, Message: "Provider não suportado."));
        }

        var fakeGoogleClientId = "fake-google-client-id";
        var fakeGoogleClientSecret = "fake-google-client-secret";

        _logger.LogInformation(
            "Simulando login do Google para {Email} com ClientId {ClientId} e ClientSecret {ClientSecret}",
            request.Email ?? "unknown",
            fakeGoogleClientId,
            fakeGoogleClientSecret);

        if (string.IsNullOrWhiteSpace(request.Token) && string.IsNullOrWhiteSpace(request.Email))
        {
            return Task.FromResult(new AuthResult(false, Message: "Token ou e-mail obrigatório."));
        }

        return Task.FromResult(new AuthResult(
            true,
            JwtToken: "fake.jwt.token.for.google.oauth2",
            Provider: request.Provider,
            Email: request.Email ?? "google-user@example.com",
            Message: "Login via Google simulado com sucesso."));
    }
}