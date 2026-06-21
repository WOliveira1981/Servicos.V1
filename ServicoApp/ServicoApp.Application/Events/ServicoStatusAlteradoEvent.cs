namespace ServicoApp.Application.Events;

/// <summary>
/// Evento disparado quando um servico muda de status.
/// </summary>
public sealed record ServicoStatusAlteradoEvent(
    Guid ServicoId,
    bool Ativo,
    DateTime DataAlteracao);
