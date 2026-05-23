namespace TaskService.Application.Abstractions;

// Um Command não retorna dados de negócio, só confirma que já executou
public interface ICommand<TResult> { }

// Uma Query nunca muda de estado, só retorna dados
public interface IQuery<TResult> { }

public interface ICommandHandler<TCommand, TResult>
  where TCommand : ICommand<TResult>
{
    System.Threading.Tasks.Task<TResult> HandleAsync(TCommand command, CancellationToken ct = default);
}


public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    System.Threading.Tasks.Task<TResult> HandleAsync(TQuery query, CancellationToken ct = default);
}
