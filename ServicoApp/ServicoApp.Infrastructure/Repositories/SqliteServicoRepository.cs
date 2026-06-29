using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<SqliteServicoRepository> _logger;

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
        catch (Exception ex)
        {
            var operationId = Guid.NewGuid();
            _logger?.LogError(ex, "Erro ao obter os serviços. OperationID={operationId}", operationId);
            throw;
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
            return await _connection.QuerySingleOrDefaultAsync<Servico>(query, new { Id = id.ToString() });
        }
        catch (Exception ex)
        {
            var operationId = Guid.NewGuid();
            _logger?.LogError(ex, "Erro ao obter o serviço. OperationID={operationId}", operationId);
            throw;
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public Task AddAsync(Servico servico)
    {
        try
        {
            const string query = @"
            INSERT INTO Servicos (Id, Nome, Descricao, Valor, Ativo)
            VALUES (@Id, @Nome, @Descricao, @Valor, @Ativo)";

            _unitOfWork.EnqueueAsync(async (connection, transaction) =>
                await connection.ExecuteAsync(query, new
                {
                    Id = servico.Id.ToString(),
                    servico.Nome,
                    servico.Descricao,
                    servico.Valor,
                    Ativo = servico.Ativo ? 1 : 0
                }, transaction));
        }
        catch (Exception ex)
        {
            var operationId = Guid.NewGuid();
            _logger?.LogError(ex, "Erro ao gravar o serviço. OperationID={operationId}", operationId);
            throw;
        }
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
        try
        {
            _unitOfWork.EnqueueAsync(async (connection, transaction) =>
                await connection.ExecuteAsync(query, new
                {
                    Id = servico.Id.ToString(),
                    servico.Nome,
                    servico.Descricao,
                    servico.Valor,
                    Ativo = servico.Ativo ? 1 : 0
                }, transaction));
        }
        catch (Exception ex)
        {
            var operationId = Guid.NewGuid();
            _logger?.LogError(ex, "Erro ao atualizar o serviço. OperationID={operationId}", operationId);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        try
        {
            _unitOfWork.EnqueueAsync(async (connection, transaction) =>
                await connection.ExecuteAsync(
                    "DELETE FROM Servicos WHERE Id = @Id",
                    new { Id = id.ToString() },
                    transaction));
        }
        catch (Exception ex)
        {
            var operationId = Guid.NewGuid();
            _logger?.LogError(ex, "Erro ao excluir o serviço. OperationID={operationId}", operationId);
            throw;
        }

        return Task.CompletedTask;
    }
}
