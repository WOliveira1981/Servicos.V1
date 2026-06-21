using ServicoApp.Domain.Entities;

namespace ServicoApp.Application.Services;

public interface IServicoAppService
{
    Task<IReadOnlyCollection<Servico>> GetAllAsync();
    Task<Servico?> GetByIdAsync(Guid id);
    Task<Servico> CreateAsync(string nome, string descricao, decimal valor);
    Task<Orcamento> CriarOrcamentoAsync(Guid servicoId, decimal valorTotal);
    Task<IReadOnlyCollection<Orcamento>> GetOrcamentosAsync();
    Task<IReadOnlyCollection<AgendaItem>> GetAgendaAsync();
    Task<Servico?> AlterarStatusAsync(Guid servicoId, bool ativo);
}
