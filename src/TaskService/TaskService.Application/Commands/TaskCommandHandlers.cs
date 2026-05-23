namespace TaskService.Application.Commands;

using TaskService.Application.Abstractions;
using TaskService.Application.DTOs;
using TaskService.Domain.Exceptions;
using TaskService.Domain.Repositories;

public sealed class CreateTaskHandler(ITaskRepository repo)
    : ICommandHandler<CreateTaskCommand, TaskDto>
{
    public async System.Threading.Tasks.Task<TaskDto> HandleAsync(
        CreateTaskCommand cmd, CancellationToken ct = default)
    {
        var task = Domain.Entities.Task.Create(cmd.UserId, cmd.Title, cmd.Description);

        await repo.AddAsync(task, ct);
        await repo.SaveChangesAsync(ct);

        task.ClearEvents();

        return TaskMapper.ToDto(task);
    }
}

public sealed class CompleteTaskHandler(ITaskRepository repo)
    : ICommandHandler<CompleteTaskCommand, TaskDto>
{
    public async System.Threading.Tasks.Task<TaskDto> HandleAsync(
        CompleteTaskCommand cmd, CancellationToken ct = default)
    {
        var task = await repo.GetByIdAsync(cmd.TaskId, ct)
            ?? throw new NotFoundException($"Task {cmd.TaskId} não encontrada.");

        task.Complete();

        await repo.SaveChangesAsync(ct);
        task.ClearEvents();

        return TaskMapper.ToDto(task);
    }
}

public sealed class DeleteTaskHandler(ITaskRepository repo)
    : ICommandHandler<DeleteTaskCommand, bool>
{
    public async System.Threading.Tasks.Task<bool> HandleAsync(
        DeleteTaskCommand cmd, CancellationToken ct = default)
    {
        var task = await repo.GetByIdAsync(cmd.TaskId, ct)
            ?? throw new NotFoundException($"Task {cmd.TaskId} não encontrada.");

        await repo.DeleteAsync(task, ct);
        await repo.SaveChangesAsync(ct);

        return true;
    }
}
