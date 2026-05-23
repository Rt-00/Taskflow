namespace TaskService.Domain.Outbox;

// Representa um evento pendente de publicação no broker
// Salvo na mesma transação que a entidade, por garantia de consistência
public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }       // ex: "TaskCreatedEvent"
    public string Payload { get; private set; }    // JSON do evento
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }  // null = pendente

    private OutboxMessage() { }

    public static OutboxMessage Create(string type, string payload) =>
        new()
        {
            Id = Guid.NewGuid(),
            Type = type,
            Payload = payload,
            CreatedAt = DateTime.UtcNow
        };

    public void MarkAsProcessed() => ProcessedAt = DateTime.UtcNow;
}
