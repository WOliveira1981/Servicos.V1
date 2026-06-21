using System.Data;
using Microsoft.Data.Sqlite;

namespace ServicoApp.Infrastructure.Persistence;

/// <summary>
/// Padrão Unit of Work: coordena várias alterações e garante que elas sejam persistidas
/// em uma mesma transação quando o método SaveChangesAsync for chamado.
/// </summary>
public sealed class SqliteUnitOfWork : Domain.Repositories.IUnitOfWork
{
    private readonly SqliteConnection _connection;
    private readonly List<Func<SqliteConnection, IDbTransaction, Task>> _pendingOperations = [];

    public SqliteUnitOfWork(SqliteConnection connection)
    {
        _connection = connection;
    }

    internal void EnqueueAsync(Func<SqliteConnection, IDbTransaction, Task> operation)
    {
        _pendingOperations.Add(operation);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_pendingOperations.Count == 0)
        {
            return 0;
        }

        var operationsToRun = _pendingOperations.Count;

        if (_connection.State != System.Data.ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken);
        }

        await using var transaction = await _connection.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var operation in _pendingOperations)
            {
                await operation(_connection, transaction);
            }

            await transaction.CommitAsync(cancellationToken);
            _pendingOperations.Clear();
            return operationsToRun;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }
        }
    }
}