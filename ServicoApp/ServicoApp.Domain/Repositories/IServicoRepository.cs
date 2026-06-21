using ServicoApp.Domain.Entities;

namespace ServicoApp.Domain.Repositories;

/// <summary>
/// Padrão Repository: abstrai a persistência da entidade de domínio.
/// </summary>
public interface IServicoRepository
{
    Task<IReadOnlyCollection<Servico>> GetAllAsync();
    Task<Servico?> GetByIdAsync(Guid id);
    Task AddAsync(Servico servico);
    Task UpdateAsync(Servico servico);
    Task DeleteAsync(Guid id);
}