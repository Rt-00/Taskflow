namespace TaskService.Application.Commands;

using TaskService.Application.Abstractions;
using TaskService.Application.DTOs;
using TaskService.Domain.Exceptions;
using TaskService.Domain.Repositories;

public sealed class CreateTaskHandler(ITaskRepository repo, IOutboxDispatcher outbox)
    : ICommandHandler<CreateTaskCommand, TaskDto>
{
    public async System.Threading.Tasks.Task<TaskDto> HandleAsync(
        CreateTaskCommand cmd, CancellationToken ct = default)
    {
        var task = Domain.Entities.Task.Create(cmd.UserId, cmd.Title, cmd.Description);

        await repo.AddAsync(task, ct);

        // Salva a tarefa + evento outbox na mesma transação
        outbox.Dispatch(task);
        await repo.SaveChangesAsync(ct);

        return TaskMapper.ToDto(task);
    }
}

public sealed class CompleteTaskHandler(ITaskRepository repo, IOutboxDispatcher outbox)
    : ICommandHandler<CompleteTaskCommand, TaskDto>
{
    public async System.Threading.Tasks.Task<TaskDto> HandleAsync(
        CompleteTaskCommand cmd, CancellationToken ct = default)
    {
        var task = await repo.GetByIdAsync(cmd.TaskId, ct)
            ?? throw new NotFoundException($"Task {cmd.TaskId} não encontrada.");

        task.Complete();

        outbox.Dispatch(task);
        await repo.SaveChangesAsync(ct);

        return TaskMapper.ToDto(task);
    }
}

public sealed class DeleteTaskHandler(ITaskRepository repo, IOutboxDispatcher outbox)
    : ICommandHandler<DeleteTaskCommand, bool>
{
    public async System.Threading.Tasks.Task<bool> HandleAsync(
        DeleteTaskCommand cmd, CancellationToken ct = default)
    {
        var task = await repo.GetByIdAsync(cmd.TaskId, ct)
            ?? throw new NotFoundException($"Task {cmd.TaskId} não encontrada.");

        await repo.DeleteAsync(task, ct);
        outbox.Dispatch(task);
        await repo.SaveChangesAsync(ct);

        return true;
    }
}
