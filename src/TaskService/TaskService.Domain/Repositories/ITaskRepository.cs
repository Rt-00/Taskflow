namespace TaskService.Domain.Repositories;

public interface ITaskRepository
{
    Task<Entities.Task?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Entities.Task>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    System.Threading.Tasks.Task AddAsync(Entities.Task task, CancellationToken ct = default);
    System.Threading.Tasks.Task SaveChangesAsync(CancellationToken ct = default);
    System.Threading.Tasks.Task DeleteAsync(Entities.Task task, CancellationToken ct = default);
}
