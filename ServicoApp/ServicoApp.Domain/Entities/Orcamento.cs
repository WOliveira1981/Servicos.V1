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

    public void Ativar() => Ativo = true;

    public void Desativar() => Ativo = false;
}