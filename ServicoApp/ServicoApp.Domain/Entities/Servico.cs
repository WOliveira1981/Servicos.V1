namespace ServicoApp.Domain.Entities;

/// <summary>
/// Entidade de domínio principal do sistema.
/// </summary>
public sealed class Servico
{
    public Guid Id { get; }
    public string Nome { get; }
    public string Descricao { get; }
    public decimal Valor { get; }
    public bool Ativo { get; private set; }

    public Servico(Guid id, string nome, string descricao, decimal valor, bool ativo = true)
    {
        Id = id;
        Nome = nome;
        Descricao = descricao;
        Valor = valor;
        Ativo = ativo;
    }

    // Este construtor auxilia o mapeamento do Dapper com tipos SQLite (texto, real e inteiro).
    public Servico(string id, string nome, string descricao, double valor, long ativo)
        : this(Guid.Parse(id), nome, descricao, (decimal)valor, ativo != 0)
    {
    }

    public void Ativar() => Ativo = true;

    public void Desativar() => Ativo = false;
}