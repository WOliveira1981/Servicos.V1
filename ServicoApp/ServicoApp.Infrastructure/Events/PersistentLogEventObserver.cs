using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServicoApp.Application.Events;
using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Repositories;

namespace ServicoApp.Infrastructure.Events;

/// <summary>
/// Padrao Observer: este componente assina todos os eventos publicados e reage
/// gravando logs. Padrao Event Sourcing: cada acao relevante e armazenada como
/// um evento imutavel, formando uma linha do tempo auditavel.
/// </summary>
public sealed class PersistentLogEventObserver : IEventObserver<object>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PersistentLogEventObserver> _logger;

    public PersistentLogEventObserver(
        IServiceScopeFactory scopeFactory,
        ILogger<PersistentLogEventObserver> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task HandleAsync(object evt, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ILogRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var entry = new LogEntry(
            Guid.NewGuid(),
            evt.GetType().Name,
            JsonSerializer.Serialize(evt),
            DateTime.UtcNow);

        await repository.AddAsync(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Evento {EventType} persistido no microsservico de logs", entry.EventType);
    }
}
