using Microsoft.Extensions.Logging;

namespace ServicoApp.Application.Events;

/// <summary>
/// Implementa o padrão Observer/Publisher para notificar ouvintes e publicar eventos em um bus mockado.
/// </summary>
public sealed class EventDispatcher : IEventPublisher
{
    private readonly IEnumerable<IEventObserver<object>> _observers;
    private readonly IServiceBus _serviceBus;
    private readonly ILogger<EventDispatcher> _logger;

    public EventDispatcher(
        IEnumerable<IEventObserver<object>> observers,
        IServiceBus serviceBus,
        ILogger<EventDispatcher> logger)
    {
        _observers = observers;
        _serviceBus = serviceBus;
        _logger = logger;
    }

    public async Task PublishAsync(object evt, CancellationToken cancellationToken = default)
    {
        var eventName = evt.GetType().Name;
        _logger.LogInformation("Disparando evento {EventName} para {ObserverCount} observadores", eventName, _observers.Count());

        foreach (var observer in _observers)
        {
            await observer.HandleAsync(evt, cancellationToken);
        }

        await _serviceBus.SendAsync(eventName, evt, cancellationToken);
    }
}