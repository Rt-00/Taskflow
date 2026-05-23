namespace TaskService.Domain.Exceptions;

// Exceção específica do domínio - diferente de erros de infraestrutura
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
