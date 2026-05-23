namespace UserService.Domain.Entities;

using UserService.Domain.Exceptions;
using UserService.Domain.ValueObjects;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // EF Core precisa de um construtor sem parâmetros (privado para proteger o domínio)
    private User() { }

    private User(Guid id, string name, Email email, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        CreatedAt = createdAt;
    }

    // Factory method - Única forma de criar um User válido
    public static User Create(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome não pode ser vazio.");

        return new User(
            id: Guid.NewGuid(),
            name: name.Trim(),
            email: Email.Create(email),
            createdAt: DateTime.UtcNow
        );
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome não pode ser vazio.");

        Name = name.Trim();
    }
}
