namespace TaskService.Application.DTOs;

public record TaskDto(
    Guid Id,
    Guid UserId,
    string Title,
    string? Description,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt
);

// Commands
public record CreateTaskCommand(Guid UserId, string Title, string? Description)
    : Abstractions.ICommand<TaskDto>;

public record CompleteTaskCommand(Guid TaskId)
    : Abstractions.ICommand<TaskDto>;

public record DeleteTaskCommand(Guid TaskId)
    : Abstractions.ICommand<bool>;

// Queries
public record GetTaskByIdQuery(Guid TaskId)
    : Abstractions.IQuery<TaskDto?>;

public record GetTasksByUserQuery(Guid UserId)
    : Abstractions.IQuery<IEnumerable<TaskDto>>;
