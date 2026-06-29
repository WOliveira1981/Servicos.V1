using ServicoApp.Domain.Entities;

namespace ServicoApp.Domain.Factories;

/// <summary>
/// Padrão Factory (GoF): encapsula a criação da entidade com regras de validação.
/// </summary>
public sealed class ServicoFactory
{
    //ERRO: private readonly ILogger<ServicoFactory> _logger; => isso quebra o padrão DDD, pois o domain fica dependente de uma infraestrura.
    //Certo: Jogar o erro pra cima, para camada de aplicação tratar, e não logar no domain.
    public Servico Create(string nome, string descricao, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do serviço é obrigatório.", nameof(nome));

        if (valor < 0)
            throw new ArgumentOutOfRangeException(nameof(valor), "O valor não pode ser negativo.");

        return new Servico(
            id: Guid.NewGuid(),
            nome: nome,
            descricao: descricao,
            valor: valor);
    }
}