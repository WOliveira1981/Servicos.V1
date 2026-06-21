using Microsoft.Extensions.Logging;
using ServicoApp.Application.Events;

namespace ServicoApp.Tests;

/// <summary>
/// Testes unitários para validar o comportamento do dispatcher e do observer.
/// </summary>
public class EventTests
{
    [Fact]
    public async Task PublishAsync_DispatchesToObserversAndServiceBus()
    {
        // Arrange
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var observer = new AuditEventObserver(loggerFactory.CreateLogger<AuditEventObserver>());
        var serviceBus = new FakeServiceBus();
        var dispatcher = new EventDispatcher(
            new[] { observer },
            serviceBus,
            loggerFactory.CreateLogger<EventDispatcher>());

        var evt = new OrcamentoCriadoEvent(
            Guid.NewGuid(),
            Guid.NewGuid(),
            250m,
            DateTime.UtcNow);

        // Act
        await dispatcher.PublishAsync(evt);

        // Assert
        Assert.Equal(evt.OrcamentoId, serviceBus.LastPublishedOrcamentoId);
    }

    // Padrão Mock / Test Double: este fake substitui o serviço externo e permite verificar
    // se a publicação do evento ocorreu sem depender de um broker real.
    private sealed class FakeServiceBus : IServiceBus
    {
        public Guid? LastPublishedOrcamentoId { get; private set; }

        public Task SendAsync(string topic, object message, CancellationToken cancellationToken = default)
        {
            if (message is OrcamentoCriadoEvent e)
            {
                LastPublishedOrcamentoId = e.OrcamentoId;
            }

            return Task.CompletedTask;
        }

        public Task<string> ReceiveAsync(string topic, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(string.Empty);
        }
    }
}