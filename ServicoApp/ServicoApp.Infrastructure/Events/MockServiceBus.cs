using ServicoApp.Application.Events;

namespace ServicoApp.Infrastructure.Events;

/// <summary>
/// Implementação mockada do Service Bus para testes e desenvolvimento.
/// </summary>
public sealed class MockServiceBus : IServiceBus
{
    private readonly Dictionary<string, object> _messages = new();

    public Task SendAsync(string topic, object message, CancellationToken cancellationToken = default)
    {
        _messages[topic] = message;
        return Task.CompletedTask;
    }

    public Task<string> ReceiveAsync(string topic, CancellationToken cancellationToken = default)
    {
        if (_messages.TryGetValue(topic, out var message))
        {
            return Task.FromResult($"{topic}:{message}");
        }

        return Task.FromResult(string.Empty);
    }
}