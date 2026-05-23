namespace TaskService.Domain.Exceptions;

// Exceção específica do domínio - diferente de erros de infraestrutura
public sealed class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
