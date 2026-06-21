using Microsoft.Extensions.DependencyInjection;
using ServicoApp.Application.Events;
using ServicoApp.Infrastructure.Events;

namespace ServicoApp.Infrastructure.Events;

/// <summary>
/// Registro dos componentes de event-driven na camada de infraestrutura.
/// </summary>
public static class EventDrivenRegistration
{
    public static IServiceCollection AddEventDriven(this IServiceCollection services)
    {
        services.AddSingleton<IServiceBus, MockServiceBus>();
        services.AddSingleton<IEventPublisher, EventDispatcher>();
        services.AddSingleton<AuditEventObserver>();
        services.AddSingleton<IEventObserver<object>, AuditEventObserver>();
        services.AddSingleton<IEventObserver<object>, PersistentLogEventObserver>();

        return services;
    }
}
