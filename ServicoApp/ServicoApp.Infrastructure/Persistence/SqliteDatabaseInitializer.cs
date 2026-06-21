using Dapper;
using Microsoft.Data.Sqlite;

namespace ServicoApp.Infrastructure.Persistence;

/// <summary>
/// Inicializa o banco SQLite executando as migrations necessárias.
/// </summary>
public sealed class SqliteDatabaseInitializer
{
    private readonly SqliteConnection _connection;

    public SqliteDatabaseInitializer(SqliteConnection connection)
    {
        _connection = connection;
    }

    public async Task InitializeAsync()
    {
        var migrationPath = Path.Combine(
            AppContext.BaseDirectory,
            "Persistence",
            "Migrations",
            "001_InitialSchema.sql");

        if (!File.Exists(migrationPath))
        {
            throw new FileNotFoundException(
                "A migration inicial não foi encontrada.",
                migrationPath);
        }

        await _connection.OpenAsync();

        try
        {
            var script = await File.ReadAllTextAsync(migrationPath);
            await _connection.ExecuteAsync(script);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}