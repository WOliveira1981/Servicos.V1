using Dapper;
using Microsoft.Data.Sqlite;
using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Repositories;

namespace ServicoApp.Infrastructure.Repositories;

/// <summary>
/// Padrão Repository com implementação em SQLite usando Dapper para orçamentos.
/// </summary>
public sealed class SqliteOrcamentoRepository : IOrcamentoRepository
{
    private readonly SqliteConnection _connection;

    public SqliteOrcamentoRepository(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<IReadOnlyCollection<Orcamento>> GetAllAsync()
    {
        await _connection.OpenAsync();
        try
        {
            var query = "SELECT Id, ServicoId, ValorTotal, DataCriacao, Ativo FROM Orcamentos";
            return (await _connection.QueryAsync<Orcamento>(query)).ToList();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public async Task<Orcamento?> GetByIdAsync(Guid id)
    {
        await _connection.OpenAsync();
        try
        {
            var query = "SELECT Id, ServicoId, ValorTotal, DataCriacao, Ativo FROM Orcamentos WHERE Id = @Id";
            return await _connection.QuerySingleOrDefaultAsync<Orcamento>(query, new { Id = id });
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public async Task<IReadOnlyCollection<Orcamento>> GetByServicoIdAsync(Guid servicoId)
    {
        await _connection.OpenAsync();
        try
        {
            var query = "SELECT Id, ServicoId, ValorTotal, DataCriacao, Ativo FROM Orcamentos WHERE ServicoId = @ServicoId";
            return (await _connection.QueryAsync<Orcamento>(query, new { ServicoId = servicoId })).ToList();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public async Task AddAsync(Orcamento orcamento)
    {
        await _connection.OpenAsync();
        try
        {
            const string query = @"
                INSERT INTO Orcamentos (Id, ServicoId, ValorTotal, DataCriacao, Ativo)
                VALUES (@Id, @ServicoId, @ValorTotal, @DataCriacao, @Ativo)";
            await _connection.ExecuteAsync(query, orcamento);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public async Task UpdateAsync(Orcamento orcamento)
    {
        await _connection.OpenAsync();
        try
        {
            const string query = @"
                UPDATE Orcamentos
                SET ServicoId = @ServicoId,
                    ValorTotal = @ValorTotal,
                    DataCriacao = @DataCriacao,
                    Ativo = @Ativo
                WHERE Id = @Id";
            await _connection.ExecuteAsync(query, orcamento);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        await _connection.OpenAsync();
        try
        {
            await _connection.ExecuteAsync(
                "DELETE FROM Orcamentos WHERE Id = @Id",
                new { Id = id });
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}