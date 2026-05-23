namespace UserService.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Persistence;

// Implementação concreta - só a Infrastructure sabe que o EF Core existe
public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    async Task IUserRepository.AddAsync(User user, CancellationToken ct) =>
        await db.Users.AddAsync(user, ct);

    async Task<IEnumerable<User>> IUserRepository.GetAllAsync(CancellationToken ct) =>
        await db.Users.ToListAsync(ct);

    Task<User?> IUserRepository.GetByEmailAsync(string email, CancellationToken ct) =>
        db.Users.FirstOrDefaultAsync(u => u.Email.Value == email.ToLowerInvariant(), ct);

    Task<User?> IUserRepository.GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    Task IUserRepository.SaveChangesAsync(CancellationToken ct) =>
        db.SaveChangesAsync(ct);
}
