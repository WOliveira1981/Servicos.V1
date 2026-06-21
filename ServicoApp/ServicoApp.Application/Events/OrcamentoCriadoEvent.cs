namespace ServicoApp.Application.Events;

/// <summary>
/// Evento disparado quando um orçamento é criado.
/// </summary>
public sealed record OrcamentoCriadoEvent(
    Guid OrcamentoId,
    Guid ServicoId,
    decimal ValorTotal,
    DateTime DataCriacao);
