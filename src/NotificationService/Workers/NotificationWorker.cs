namespace NotificationService.Workers;

using System.Text;
using System.Text.Json;
using NotificationService.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public sealed class NotificationWorker(ILogger<NotificationWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "taskflow",
            Password = "taskflow",
        };

        using var connection = await factory.CreateConnectionAsync(ct);
        using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

        // Declara o mesmo exchange do publisher
        await channel.ExchangeDeclareAsync(
            exchange: "taskflow.events",
            type: ExchangeType.Topic,
            durable: true,
            cancellationToken: ct);

        // Fila exclusiva deste serviço
        var queue = await channel.QueueDeclareAsync(
            queue: "notification-service",
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: ct);

        // Binding: escuta task.created e task.completed
        await channel.QueueBindAsync(queue.QueueName, "taskflow.events", "task.created", cancellationToken: ct);
        await channel.QueueBindAsync(queue.QueueName, "taskflow.events", "task.completed", cancellationToken: ct);

        // Processa 1 mensagem por vez — evita sobrecarga
        await channel.BasicQosAsync(0, 1, false, ct);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.Span);
                var routingKey = ea.RoutingKey;

                logger.LogInformation("Evento recebido: {RoutingKey}", routingKey);

                await HandleAsync(routingKey, body, ct);

                // ACK — confirma que processou
                await channel.BasicAckAsync(ea.DeliveryTag, false, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar evento.");
                // NACK com requeue=false — manda para dead-letter (fase futura)
                await channel.BasicNackAsync(ea.DeliveryTag, false, false, ct);
            }
        };

        await channel.BasicConsumeAsync(queue.QueueName, autoAck: false, consumer, ct);

        logger.LogInformation("Notification Service aguardando eventos...");

        // Mantém o worker vivo
        await Task.Delay(Timeout.Infinite, ct);
    }

    private Task HandleAsync(string routingKey, string body, CancellationToken ct)
    {
        switch (routingKey)
        {
            case "task.created":
                {
                    var evt = JsonSerializer.Deserialize<TaskCreatedEvent>(body);
                    if (evt is null) break;

                    // Em produção: enviar e-mail, push notification, webhook...
                    logger.LogInformation(
                        "[NOTIFICAÇÃO] Nova tarefa criada: '{Title}' para usuário {UserId}",
                        evt.Title, evt.UserId);

                    break;
                }
            case "task.completed":
                {
                    var evt = JsonSerializer.Deserialize<TaskCompletedEvent>(body);
                    if (evt is null) break;

                    logger.LogInformation(
                        "[NOTIFICAÇÃO] Tarefa {TaskId} concluída pelo usuário {UserId}",
                        evt.TaskId, evt.UserId);

                    break;
                }
            default:
                logger.LogWarning("Evento desconhecido: {RoutingKey}", routingKey);
                break;
        }

        return Task.CompletedTask;
    }
}
