using ServicoApp.Domain.Entities;

namespace ServicoApp.Application.Services;

public interface ILogService
{
    Task<IReadOnlyCollection<LogEntry>> GetHistoricoAsync();
}
