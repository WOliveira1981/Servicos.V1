namespace ServicoApp.Application.Services;

public sealed record AgendaItem(
    Guid Id,
    string Titulo,
    DateTime Data,
    string Status);
