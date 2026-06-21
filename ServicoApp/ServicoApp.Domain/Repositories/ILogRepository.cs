using ServicoApp.Domain.Entities;

namespace ServicoApp.Domain.Repositories;

/// <summary>
/// Padrao Repository: isola a persistencia do microsservico de logs.
/// </summary>
public interface ILogRepository
{
    Task<IReadOnlyCollection<LogEntry>> GetAllAsync();
    Task AddAsync(LogEntry entry);
}
