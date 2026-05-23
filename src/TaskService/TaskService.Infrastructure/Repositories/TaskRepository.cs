using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Repositories;
using TaskService.Infrastructure.Persistence;

namespace TaskService.Infrastructure.Repositories;

public sealed class TaskRepository(AppDbContext db) : ITaskRepository
{
    public async System.Threading.Tasks.Task AddAsync(
        Domain.Entities.Task task,
        CancellationToken ct = default) => await db.Tasks.AddAsync(task, ct);

    public System.Threading.Tasks.Task DeleteAsync(
        Domain.Entities.Task task,
        CancellationToken ct = default)
    {
        db.Tasks.Remove(task);
        return System.Threading.Tasks.Task.CompletedTask;
    }

    public System.Threading.Tasks.Task<Domain.Entities.Task?> GetByIdAsync(Guid id,
        CancellationToken ct = default) => db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IEnumerable<Domain.Entities.Task>> GetByUserIdAsync(
        Guid userId,
        CancellationToken ct = default) =>
        await db.Tasks
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);

    public System.Threading.Tasks.Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
