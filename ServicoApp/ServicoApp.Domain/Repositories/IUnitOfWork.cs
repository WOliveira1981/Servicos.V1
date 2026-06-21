namespace ServicoApp.Domain.Repositories;

/// <summary>
/// Padrão Unit of Work: coordena várias alterações e garante que elas sejam persistidas de forma consistente.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}