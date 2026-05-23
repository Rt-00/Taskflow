using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using TaskService.Infrastructure.Persistence;

namespace TaskService.Infrastructure.Messaging;

// BackgroundService roda em paralelo com a API, não bloqueia os requests
public sealed class OutboxPublisher(
    IServiceScopeFactory scopeFactory,
    ILogger<OutboxPublisher> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        logger.LogInformation("Outbox Publisher iniciado.");

        while (!ct.IsCancellationRequested)
        {
            try
            {
                await PublishPendingAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro no Outbox Publisher.");
            }

            // Pooling a cada 5 segundos
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
        }
    }

    private async Task PublishPendingAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Busca apenas mensagens não processadas
        var pending = await db.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(50)
            .ToListAsync(ct);

        if (!pending.Any()) return;

        // Conecta ao RabbitMQ
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "taskflow",
            Password = "taskflow"
        };
        using var connection = await factory.CreateConnectionAsync(ct);
        using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

        // Declara exchange do tipo topic, permite roteamento por padrão
        await channel.ExchangeDeclareAsync(
            exchange: "taskflow.events",
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct
        );

        foreach (var msg in pending)
        {
            var body = Encoding.UTF8.GetBytes(msg.Payload);

            // Routing key = nome do evento em snake_case = "task.created", "task.completed"
            var routingKey = ToRoutingKey(msg.Type);

            var props = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(
                exchange: "taskflow.events",
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: ct
            );

            msg.MarkAsProcessed();
            logger.LogInformation("Publicado: {Type} ({Id})", msg.Type, msg.Id);
        }

        await db.SaveChangesAsync(ct);
    }

    private static string ToRoutingKey(string eventType) => eventType switch
    {
        "TaskCreatedEvent" => "task.created",
        "TaskCompletedEvent" => "task.completed",
        "TaskDeletedEvent" => "task.deleted",
        _ => eventType.ToLowerInvariant()
    };
}
