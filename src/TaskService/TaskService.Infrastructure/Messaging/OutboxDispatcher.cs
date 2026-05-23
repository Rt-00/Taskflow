namespace TaskService.Infrastructure.Messaging;

using TaskService.Application.Abstractions;
using TaskService.Infrastructure.Persistence;
using TaskService.Domain.Entities;
using TaskService.Domain.Outbox;
using System.Text.Json;


// Convert domain events em OutboxMessages e salva no banco
// Chamado dentro dos command handlers, mesma transação
public sealed class OutboxDispatcher(AppDbContext db): IOutboxDispatcher
{
    public void Dispatch(Task task)
    {
        foreach (var evt in task.DomainEvents)
        {
            var msg = OutboxMessage.Create(
                type: evt.GetType().Name,
                payload: JsonSerializer.Serialize(evt, evt.GetType())
            );

            db.OutboxMessages.Add(msg);
        }

        task.ClearEvents();
    }
}
