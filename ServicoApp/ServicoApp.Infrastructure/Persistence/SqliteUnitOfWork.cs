using Microsoft.Data.Sqlite;

namespace ServicoApp.Infrastructure.Persistence;

/// <summary>
/// Padrão Unit of Work: mantém o contexto de persistência compartilhado para os repositórios.
/// </summary>
public sealed class SqliteUnitOfWork : Domain.Repositories.IUnitOfWork
{
    private readonly SqliteConnection _connection;

    public SqliteUnitOfWork(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken);
        }

        // Em uma implementação completa, aqui seria criado e confirmado um transaction scope.
        await _connection.CloseAsync();
        return 1;
    }
}