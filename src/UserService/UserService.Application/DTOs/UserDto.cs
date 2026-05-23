namespace UserService.Application.DTOs;

// DTOs transitam entre camadas — nunca exponha a entidade de domínio diretamente
public record UserDto(Guid Id, string Name, string Email, DateTime CreatedAt);

public record CreateUserRequest(string Name, string Email);

public record UpdateUserRequest(string Name);
