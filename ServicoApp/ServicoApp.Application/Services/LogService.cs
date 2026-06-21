using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Repositories;

namespace ServicoApp.Application.Services;

/// <summary>
/// Microsservico de logs: expoe a leitura da trilha de eventos registrada pelos observers.
/// </summary>
public sealed class LogService : ILogService
{
    private readonly ILogRepository _repository;

    public LogService(ILogRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyCollection<LogEntry>> GetHistoricoAsync() => _repository.GetAllAsync();
}
