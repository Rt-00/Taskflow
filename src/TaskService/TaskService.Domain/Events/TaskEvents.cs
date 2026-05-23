namespace TaskService.Domain.Events;


// Eventos de domínio são fatos imutáveis, algo que JÁ aconteceu
public sealed record TaskCreatedEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid TaskId,
    Guid UserId,
    string Title
) : IDomainEvent;

public sealed record TaskCompletedEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid TaskId,
    Guid UserId
) : IDomainEvent;

public sealed record TaskDeletedEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid TaskId,
    Guid UserId
) : IDomainEvent;
