using UserService.Application.DTOs;
using UserService.Domain.Entities;
using UserService.Domain.Repositories;

namespace UserService.Application.UseCases;

public sealed class CreateUserUseCase(IUserRepository repo)
{
    public async Task<UserDto> ExecuteAsync(
        CreateUserRequest req,
        CancellationToken ct = default)
    {
        var existing = await repo.GetByEmailAsync(req.Email, ct);

        if (existing is not null)
            throw new InvalidOperationException($"E-mail '{req.Email}' já está em uso.");

        var user = User.Create(req.Name, req.Email);

        await repo.AddAsync(user, ct);
        await repo.SaveChangesAsync(ct);

        return toDto(user);
    }

    private static UserDto toDto(User u) => new(u.Id, u.Name, u.Email.Value, u.CreatedAt);
}
