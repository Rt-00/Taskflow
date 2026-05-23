namespace TaskService.Application.Queries;

using TaskService.Application.Abstractions;
using TaskService.Application.Commands;
using TaskService.Application.DTOs;
using TaskService.Domain.Repositories;

public sealed class GetTaskByIdHandler(ITaskRepository repo)
    : IQueryHandler<GetTaskByIdQuery, TaskDto?>
{
    public async System.Threading.Tasks.Task<TaskDto?> HandleAsync(
        GetTaskByIdQuery query, CancellationToken ct = default)
    {
        var task = await repo.GetByIdAsync(query.TaskId, ct);
        return task is null ? null : TaskMapper.ToDto(task);
    }
}

public sealed class GetTasksByUserHandler(ITaskRepository repo)
    : IQueryHandler<GetTasksByUserQuery, IEnumerable<TaskDto>>
{
    public async System.Threading.Tasks.Task<IEnumerable<TaskDto>> HandleAsync(
        GetTasksByUserQuery query, CancellationToken ct = default)
    {
        var tasks = await repo.GetByUserIdAsync(query.UserId, ct);
        return tasks.Select(TaskMapper.ToDto);
    }
}
