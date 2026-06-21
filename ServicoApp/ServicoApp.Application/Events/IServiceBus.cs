namespace ServicoApp.Application.Events;

/// <summary>
/// Interface mockada para representar um Service Bus genérico.
/// </summary>
public interface IServiceBus
{
    Task SendAsync(string topic, object message, CancellationToken cancellationToken = default);
    Task<string> ReceiveAsync(string topic, CancellationToken cancellationToken = default);
}