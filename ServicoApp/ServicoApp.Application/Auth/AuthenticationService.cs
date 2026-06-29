using Microsoft.Extensions.Logging;

namespace ServicoApp.Application.Auth;

/// <summary>
/// Orquestra a estratégia de autenticação escolhida para a requisição.
/// </summary>
public sealed class AuthenticationService
{
    private readonly IEnumerable<IAuthenticationStrategy> _strategies;
    private readonly JwtTokenService _jwtTokenService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IEnumerable<IAuthenticationStrategy> strategies,
        JwtTokenService jwtTokenService,
        ILogger<AuthenticationService> logger)
    {
        _strategies = strategies;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<AuthResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        var strategy = _strategies.FirstOrDefault(s => s.GetType().Name.Contains(request.Provider, StringComparison.OrdinalIgnoreCase));

        if (strategy is null)
        {
            var operationId = Guid.NewGuid();
            _logger.LogWarning("Nenhuma estratégia encontrada para provider {Provider}. OperationID={operationId}", request.Provider, operationId);
            return new AuthResult(false, Message: "Provider não configurado.");
        }

        var result = await strategy.AuthenticateAsync(request, cancellationToken);

        if (result.IsAuthenticated && !string.IsNullOrWhiteSpace(result.Email))
        {
            var token = _jwtTokenService.GenerateToken(result.Email, result.Provider ?? request.Provider);
            return result with { JwtToken = token };
        }

        return result;
    }
}