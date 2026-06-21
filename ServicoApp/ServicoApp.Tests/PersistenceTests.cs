using Dapper;
using Microsoft.Data.Sqlite;
using ServicoApp.Domain.Entities;
using ServicoApp.Infrastructure.Persistence;
using ServicoApp.Infrastructure.Repositories;

namespace ServicoApp.Tests;

/// <summary>
/// Testes de integração para verificar persistência com SQLite e Unit of Work.
/// </summary>
public class PersistenceTests
{
    [Fact]
    public async Task SaveChangesAsync_PersistsServicoAndAllowsReadback()
    {
        var dbPath = Path.Combine(Path.GetTempPath(), $"servicoapp-tests-{Guid.NewGuid():N}.db");

        try
        {
            await using var connection = new SqliteConnection($"Data Source={dbPath}");
            await connection.OpenAsync();

            await connection.ExecuteAsync(@"
                CREATE TABLE Servicos (
                    Id TEXT PRIMARY KEY,
                    Nome TEXT NOT NULL,
                    Descricao TEXT NOT NULL,
                    Valor REAL NOT NULL,
                    Ativo INTEGER NOT NULL
                );");

            var unitOfWork = new SqliteUnitOfWork(connection);
            var repository = new SqliteServicoRepository(connection, unitOfWork);
            var servico = new Servico(Guid.NewGuid(), "Teste", "Descrição", 100m);

            await repository.AddAsync(servico);
            await unitOfWork.SaveChangesAsync();

            var saved = await repository.GetByIdAsync(servico.Id);

            Assert.NotNull(saved);
            Assert.Equal(servico.Nome, saved!.Nome);
            Assert.Equal(servico.Valor, saved.Valor);
        }
        finally
        {
            try
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                }
            }
            catch
            {
                // Ignora falhas de limpeza em ambientes com lock do SQLite.
            }
        }
    }
}