namespace TaskService.Application.Commands;

using TaskService.Application.DTOs;

// Mapper interno ao namespace Application — acessível por commands e queries
internal static class TaskMapper
{
    public static TaskDto ToDto(Domain.Entities.Task t) =>
        new(t.Id, t.UserId, t.Title, t.Description,
            t.Status.ToString(), t.CreatedAt, t.CompletedAt);
}
