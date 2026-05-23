using UserService.Application.DTOs;
using UserService.Domain.Repositories;

namespace UserService.Application.UseCases;

public sealed class GetUserUseCase(IUserRepository repo)
{
    public async Task<UserDto?> ExecuteAsync(Guid id, CancellationToken ct = default)
    {
        var user = await repo.GetByIdAsync(id, ct);

        if (user is null) return null;

        return new UserDto(user.Id, user.Name, user.Email.Value, user.CreatedAt);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        var users = await repo.GetAllAsync(ct);
        return users.Select(u => new UserDto(u.Id, u.Name, u.Email.Value, u.CreatedAt));
    }
}
