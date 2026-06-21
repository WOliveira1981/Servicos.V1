using Dapper;
using Microsoft.Data.Sqlite;
using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Repositories;
using ServicoApp.Infrastructure.Persistence;

namespace ServicoApp.Infrastructure.Repositories;

/// <summary>
/// Padrão Repository: encapsula o acesso aos dados de Serviço e mantém a aplicação
/// desacoplada do detalhe físico do SQLite/Dapper.
/// </summary>
public sealed class SqliteServicoRepository : IServicoRepository
{
    private readonly SqliteConnection _connection;
    private readonly SqliteUnitOfWork _unitOfWork;

    public SqliteServicoRepository(SqliteConnection connection, SqliteUnitOfWork unitOfWork)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<Servico>> GetAllAsync()
    {
        await _connection.OpenAsync();
        try
        {
            const string query = "SELECT Id, Nome, Descricao, Valor, Ativo FROM Servicos";
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
            const string query = "SELECT Id, Nome, Descricao, Valor, Ativo FROM Servicos WHERE Id = @Id";
            return await _connection.QuerySingleOrDefaultAsync<Servico>(query, new { Id = id });
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public Task AddAsync(Servico servico)
    {
        const string query = @"
            INSERT INTO Servicos (Id, Nome, Descricao, Valor, Ativo)
            VALUES (@Id, @Nome, @Descricao, @Valor, @Ativo)";

        _unitOfWork.EnqueueAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(query, servico, transaction));

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Servico servico)
    {
        const string query = @"
            UPDATE Servicos
            SET Nome = @Nome,
                Descricao = @Descricao,
                Valor = @Valor,
                Ativo = @Ativo
            WHERE Id = @Id";

        _unitOfWork.EnqueueAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(query, servico, transaction));

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _unitOfWork.EnqueueAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(
                "DELETE FROM Servicos WHERE Id = @Id",
                new { Id = id },
                transaction));

        return Task.CompletedTask;
    }
}