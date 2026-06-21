namespace ServicoApp.Application.Auth;

/// <summary>
/// Padrão Strategy: permite trocar a estratégia de autenticação sem alterar a API.
/// Cada implementação decide como validar o usuário e emitir a resposta apropriada.
/// </summary>
public interface IAuthenticationStrategy
{
    Task<AuthResult> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken = default);
}

public sealed record AuthenticationRequest(
    string Provider,
    string? Token,
    string? Email,
    string? Name = null);

public sealed record AuthResult(
    bool IsAuthenticated,
    string? JwtToken = null,
    string? Provider = null,
    string? Email = null,
    string? Message = null);
