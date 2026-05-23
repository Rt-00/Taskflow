namespace TaskService.Domain.Entities;

using TaskService.Domain.Events;
using TaskService.Domain.Exceptions;

public enum TaskStatus { Pending, InProgress, Completed };

public sealed class Task
{
    private readonly List<IDomainEvent> _events = [];

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Leitura dos eventos acumulados (sem expor a lista mutável)
    public IReadOnlyList<IDomainEvent> DomainEvents => _events.AsReadOnly();

    private Task() { }

    public static Task Create(Guid userId, string title, string? description = null)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId inválido");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Título não pode ser vazio.");

        var task = new Task
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title.Trim(),
            Description = description?.Trim(),
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Registra o evento, será despachado após persistir
        task._events.Add(new TaskCreatedEvent(
            Guid.NewGuid(), DateTime.UtcNow, task.Id, task.UserId, task.Title));

        return task;
    }

    public void Complete()
    {
        if (Status == TaskStatus.Completed)
            throw new DomainException("Tarefa já está concluída");

        Status = TaskStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        _events.Add(new TaskCompletedEvent(
            Guid.NewGuid(), DateTime.UtcNow, Id, UserId));
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Título não pode ser vazio.");

        Title = title.Trim();
    }

    // Limpa eventos após o despacho para não re-publicar
    public void ClearEvents() => _events.Clear();
}
