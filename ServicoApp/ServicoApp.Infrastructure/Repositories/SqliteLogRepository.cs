using Dapper;
using Microsoft.Data.Sqlite;
using ServicoApp.Domain.Entities;
using ServicoApp.Domain.Repositories;
using ServicoApp.Infrastructure.Persistence;

namespace ServicoApp.Infrastructure.Repositories;

/// <summary>
/// Repository do microsservico de logs. Ele grava eventos sem sobrescrever registros,
/// preservando a sequencia historica usada pelo Event Sourcing.
/// </summary>
public sealed class SqliteLogRepository : ILogRepository
{
    private readonly SqliteConnection _connection;
    private readonly SqliteUnitOfWork _unitOfWork;

    public SqliteLogRepository(SqliteConnection connection, SqliteUnitOfWork unitOfWork)
    {
        _connection = connection;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyCollection<LogEntry>> GetAllAsync()
    {
        await _connection.OpenAsync();
        try
        {
            const string query = "SELECT Id, EventType, Payload, OccurredAt FROM Logs ORDER BY OccurredAt DESC";
            return (await _connection.QueryAsync<LogEntry>(query)).ToList();
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    public Task AddAsync(LogEntry entry)
    {
        const string query = @"
            INSERT INTO Logs (Id, EventType, Payload, OccurredAt)
            VALUES (@Id, @EventType, @Payload, @OccurredAt)";

        _unitOfWork.EnqueueAsync(async (connection, transaction) =>
            await connection.ExecuteAsync(query, new
            {
                Id = entry.Id.ToString(),
                entry.EventType,
                entry.Payload,
                entry.OccurredAt
            }, transaction));

        return Task.CompletedTask;
    }
}
