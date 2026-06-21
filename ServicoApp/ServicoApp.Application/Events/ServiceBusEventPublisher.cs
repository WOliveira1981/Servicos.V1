using Microsoft.Extensions.Logging;

namespace ServicoApp.Application.Events;

/// <summary>
/// Implementação mockada de publisher usando uma abstração de Service Bus.
/// </summary>
public sealed class ServiceBusEventPublisher : IEventPublisher
{
    private readonly IServiceBus _serviceBus;
    private readonly ILogger<ServiceBusEventPublisher> _logger;

    public ServiceBusEventPublisher(IServiceBus serviceBus, ILogger<ServiceBusEventPublisher> logger)
    {
        _serviceBus = serviceBus;
        _logger = logger;
    }

    public async Task PublishAsync(object evt, CancellationToken cancellationToken = default)
    {
        var topic = evt.GetType().Name;
        _logger.LogInformation("Publicando evento {EventName} no tópico {Topic}", evt.GetType().Name, topic);
        await _serviceBus.SendAsync(topic, evt, cancellationToken);
    }
}