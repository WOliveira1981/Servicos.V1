using ServicoApp.Application.Auth;
using ServicoApp.Domain.Entities;

namespace ServicoApp.Application.Gateway;

/// <summary>
/// Contrato do API Gateway para integrar múltiplos fluxos da aplicação em um ponto único.
/// </summary>
public interface IApiGatewayService
{
    Task<IReadOnlyCollection<Servico>> GetAllAsync();
    Task<Servico?> GetByIdAsync(Guid id);
    Task<Servico> CreateAsync(string nome, string descricao, decimal valor);
    Task<AuthResult> LoginAsync(AuthenticationRequest request);
}