namespace UserService.Domain.Repositories;

using UserService.Domain.Entities;

// Interface definida no Domain - implementação fica na Infrastructure
// Isso é o Dependency Inversion Principe em ação
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
