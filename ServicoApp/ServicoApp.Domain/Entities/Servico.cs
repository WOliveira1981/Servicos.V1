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

    public void Ativar() => Ativo = true;

    public void Desativar() => Ativo = false;
}