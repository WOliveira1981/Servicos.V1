using ServicoApp.Application.Auth;
using ServicoApp.Application.Services;
using ServicoApp.Domain.Entities;

namespace ServicoApp.Application.Gateway;

/// <summary>
/// Contrato do API Gateway para integrar multiplos fluxos da aplicacao em um ponto unico.
/// </summary>
public interface IApiGatewayService
{
    Task<IReadOnlyCollection<Servico>> GetAllAsync();
    Task<Servico?> GetByIdAsync(Guid id);
    Task<Servico> CreateAsync(string nome, string descricao, decimal valor);
    Task<AuthResult> LoginAsync(AuthenticationRequest request);
    Task<Orcamento> CriarOrcamentoAsync(Guid servicoId, decimal valorTotal);
    Task<IReadOnlyCollection<Orcamento>> GetOrcamentosAsync();
    Task<IReadOnlyCollection<AgendaItem>> GetAgendaAsync();
    Task<IReadOnlyCollection<LogEntry>> GetHistoricoAsync();
    Task<Servico?> AlterarStatusAsync(Guid servicoId, bool ativo);
}
