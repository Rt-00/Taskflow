namespace NotificationService.Events;

// Espelha os eventos do TaskService — desacoplado por contrato (não por referência)
public sealed record TaskCreatedEvent(
    Guid EventId, DateTime OccurredAt,
    Guid TaskId, Guid UserId, string Title);

public sealed record TaskCompletedEvent(
    Guid EventId, DateTime OccurredAt,
    Guid TaskId, Guid UserId);
