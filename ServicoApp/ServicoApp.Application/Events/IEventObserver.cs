namespace ServicoApp.Application.Events;

/// <summary>
/// Contrato para observers que reagem a eventos de domínio.
/// </summary>
public interface IEventObserver<in TEvent>
{
    Task HandleAsync(TEvent evt, CancellationToken cancellationToken = default);
}