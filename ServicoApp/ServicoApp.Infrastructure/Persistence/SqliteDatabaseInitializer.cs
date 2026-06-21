using Dapper;
using Microsoft.Data.Sqlite;

namespace ServicoApp.Infrastructure.Persistence;

/// <summary>
/// Inicializa o banco SQLite executando todas as migrations encontradas na pasta de migrações.
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
        var migrationsDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "Persistence",
            "Migrations");

        if (!Directory.Exists(migrationsDirectory))
        {
            throw new DirectoryNotFoundException(
                $"O diretório de migrations não foi encontrado: {migrationsDirectory}");
        }

        var migrationFiles = Directory.GetFiles(migrationsDirectory, "*.sql")
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (migrationFiles.Length == 0)
        {
            throw new FileNotFoundException(
                $"Nenhuma migration foi encontrada em {migrationsDirectory}.");
        }

        await _connection.OpenAsync();

        try
        {
            await _connection.ExecuteAsync("PRAGMA foreign_keys = ON;");

            foreach (var migrationFile in migrationFiles)
            {
                var script = await File.ReadAllTextAsync(migrationFile);
                await _connection.ExecuteAsync(script);
            }
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }
}