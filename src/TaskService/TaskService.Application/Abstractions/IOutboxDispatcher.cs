namespace TaskService.Application.Abstractions;

using TaskService.Domain.Entities;

// Interface no Application — implementação fica na Infrastructure
// Mesmo princípio do ITaskRepository
public interface IOutboxDispatcher
{
    void Dispatch(Task task);
}
