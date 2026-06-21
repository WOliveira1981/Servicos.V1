using Microsoft.Extensions.Logging;

namespace ServicoApp.Application.Events;

/// <summary>
/// Observer responsável por registrar auditoria dos eventos publicados.
/// </summary>
public sealed class AuditEventObserver : IEventObserver<object>
{
    private readonly ILogger<AuditEventObserver> _logger;

    public AuditEventObserver(ILogger<AuditEventObserver> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(object evt, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "AUDITORIA - Evento {EventName} recebido em {Timestamp} com payload {Payload}",
            evt.GetType().Name,
            DateTime.UtcNow,
            evt);

        return Task.CompletedTask;
    }
}