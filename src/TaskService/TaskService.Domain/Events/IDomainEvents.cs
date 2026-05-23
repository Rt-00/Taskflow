namespace TaskService.Domain.Events;

// Marcador. Todo domain event implementa essa interface
public interface IDomainEvent
{
    Guid EventId { get; }
}
