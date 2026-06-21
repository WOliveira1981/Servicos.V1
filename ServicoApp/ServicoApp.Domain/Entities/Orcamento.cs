namespace ServicoApp.Domain.Entities;

/// <summary>
/// Entidade que representa o orçamento associado a um serviço.
/// </summary>
public sealed class Orcamento
{
    public Guid Id { get; }
    public Guid ServicoId { get; }
    public decimal ValorTotal { get; }
    public DateTime DataCriacao { get; }
    public bool Ativo { get; private set; }

    public Orcamento(Guid id, Guid servicoId, decimal valorTotal, DateTime dataCriacao, bool ativo = true)
    {
        Id = id;
        ServicoId = servicoId;
        ValorTotal = valorTotal;
        DataCriacao = dataCriacao;
        Ativo = ativo;
    }

    // Este construtor auxilia o mapeamento do Dapper com os tipos retornados pelo SQLite.
    public Orcamento(string id, string servicoId, double valorTotal, string dataCriacao, long ativo)
        : this(Guid.Parse(id), Guid.Parse(servicoId), (decimal)valorTotal, DateTime.Parse(dataCriacao), ativo != 0)
    {
    }

    public Orcamento(string id, string servicoId, string valorTotal, string dataCriacao, long ativo)
        : this(Guid.Parse(id), Guid.Parse(servicoId), decimal.Parse(valorTotal), DateTime.Parse(dataCriacao), ativo != 0)
    {
    }

    public Orcamento(string id, string servicoId, long valorTotal, string dataCriacao, long ativo)
        : this(Guid.Parse(id), Guid.Parse(servicoId), valorTotal, DateTime.Parse(dataCriacao), ativo != 0)
    {
    }

    public void Ativar() => Ativo = true;

    public void Desativar() => Ativo = false;
}
