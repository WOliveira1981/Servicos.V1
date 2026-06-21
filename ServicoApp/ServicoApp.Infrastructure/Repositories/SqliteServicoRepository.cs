using Dapper;
using Microsoft.Data.Sqlite;
using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Repositories;

namespace ServicoApp.Infrastructure.Repositories;

/// <summary>
/// Padrão Repository com implementação em SQLite usando Dapper.
/// </summary>
public sealed class SqliteServicoRepository : IServicoRepository
{
    private readonly SqliteConnection _connection;

    public SqliteServicoRepository(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<IReadOnlyCollection<Servico>> GetAllAsync()
    {
        await _connection.OpenAsync();
        try
        {
            var query = "SELECT Id, Nome, Descricao, Valor, Ativo FROM Servicos";
            return (await _connection.QueryAsync<Servico>(query)).ToList();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public async Task<Servico?> GetByIdAsync(Guid id)
    {
        await _connection.OpenAsync();
        try
        {
            var query = "SELECT Id, Nome, Descricao, Valor, Ativo FROM Servicos WHERE Id = @Id";
            return await _connection.QuerySingleOrDefaultAsync<Servico>(query, new { Id = id });
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public async Task AddAsync(Servico servico)
    {
        await _connection.OpenAsync();
        try
        {
            const string query = @"
                INSERT INTO Servicos (Id, Nome, Descricao, Valor, Ativo)
                VALUES (@Id, @Nome, @Descricao, @Valor, @Ativo)";
            await _connection.ExecuteAsync(query, servico);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public async Task UpdateAsync(Servico servico)
    {
        await _connection.OpenAsync();
        try
        {
            const string query = @"
                UPDATE Servicos
                SET Nome = @Nome,
                    Descricao = @Descricao,
                    Valor = @Valor,
                    Ativo = @Ativo
                WHERE Id = @Id";
            await _connection.ExecuteAsync(query, servico);
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
                "DELETE FROM Servicos WHERE Id = @Id",
                new { Id = id });
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}