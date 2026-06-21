namespace ServicoApp.Domain.Entities;

/// <summary>
/// Registro imutavel de evento do sistema. Ele compoe o microsservico de logs e
/// mantem a trilha historica usada pelo Event Sourcing.
/// </summary>
public sealed class LogEntry
{
    public Guid Id { get; }
    public string EventType { get; }
    public string Payload { get; }
    public DateTime OccurredAt { get; }

    public LogEntry(Guid id, string eventType, string payload, DateTime occurredAt)
    {
        Id = id;
        EventType = eventType;
        Payload = payload;
        OccurredAt = occurredAt;
    }

    public LogEntry(string id, string eventType, string payload, string occurredAt)
        : this(Guid.Parse(id), eventType, payload, DateTime.Parse(occurredAt))
    {
    }
}
