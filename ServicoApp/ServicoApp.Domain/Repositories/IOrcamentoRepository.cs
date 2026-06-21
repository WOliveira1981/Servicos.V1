using ServicoApp.Domain.Entities;

namespace ServicoApp.Domain.Repositories;

/// <summary>
/// Padrão Repository: abstrai a persistência da entidade de orçamento.
/// </summary>
public interface IOrcamentoRepository
{
    Task<IReadOnlyCollection<Orcamento>> GetAllAsync();
    Task<Orcamento?> GetByIdAsync(Guid id);
    Task<IReadOnlyCollection<Orcamento>> GetByServicoIdAsync(Guid servicoId);
    Task AddAsync(Orcamento orcamento);
    Task UpdateAsync(Orcamento orcamento);
    Task DeleteAsync(Guid id);
}