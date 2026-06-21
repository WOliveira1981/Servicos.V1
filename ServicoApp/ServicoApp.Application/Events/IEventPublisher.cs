namespace ServicoApp.Application.Events;

/// <summary>
/// Abstração para publicação de eventos no estilo event-driven.
/// </summary>
public interface IEventPublisher
{
    Task PublishAsync(object evt, CancellationToken cancellationToken = default);
}