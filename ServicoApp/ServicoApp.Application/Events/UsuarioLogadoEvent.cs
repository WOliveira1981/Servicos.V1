namespace ServicoApp.Application.Events;

/// <summary>
/// Evento disparado quando um usuário realiza login.
/// </summary>
public sealed record UsuarioLogadoEvent(
    Guid UsuarioId,
    string NomeUsuario,
    DateTime DataLogin);
