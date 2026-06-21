using Dapper;
using Microsoft.Data.Sqlite;
using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Repositories;
using ServicoApp.Infrastructure.Persistence;

namespace ServicoApp.Infrastructure.Repositories;

/// <summary>
/// Padrão Repository: separa a lógica de consulta e escrita de Orçamento do restante da aplicação.
/// </summary>
public sealed class SqliteOrcamentoRepository : IOrcamentoRepository
{
    private readonly SqliteConnection _connection;
    private readonly SqliteUnitOfWork _unitOfWork;

    public SqliteOrcamentoRepository(SqliteConnection connection, SqliteUnitOfWork unitOfWork)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<Orcamento>> GetAllAsync()
    {
        await _connection.OpenAsync();
        try
        {
            const string query = "SELECT Id, ServicoId, ValorTotal, DataCriacao, Ativo FROM Orcamentos";
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
            const string query = "SELECT Id, ServicoId, ValorTotal, DataCriacao, Ativo FROM Orcamentos WHERE Id = @Id";
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
            const string query = "SELECT Id, ServicoId, ValorTotal, DataCriacao, Ativo FROM Orcamentos WHERE ServicoId = @ServicoId";
            return (await _connection.QueryAsync<Orcamento>(query, new { ServicoId = servicoId })).ToList();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public Task AddAsync(Orcamento orcamento)
    {
        const string query = @"
            INSERT INTO Orcamentos (Id, ServicoId, ValorTotal, DataCriacao, Ativo)
            VALUES (@Id, @ServicoId, @ValorTotal, @DataCriacao, @Ativo)";

        _unitOfWork.EnqueueAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(query, orcamento, transaction));

        return Task.CompletedTask;
    }

    public Task UpdateAsync(Orcamento orcamento)
    {
        const string query = @"
            UPDATE Orcamentos
            SET ServicoId = @ServicoId,
                ValorTotal = @ValorTotal,
                DataCriacao = @DataCriacao,
                Ativo = @Ativo
            WHERE Id = @Id";

        _unitOfWork.EnqueueAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(query, orcamento, transaction));

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _unitOfWork.EnqueueAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(
                "DELETE FROM Orcamentos WHERE Id = @Id",
                new { Id = id },
                transaction));

        return Task.CompletedTask;
    }
}