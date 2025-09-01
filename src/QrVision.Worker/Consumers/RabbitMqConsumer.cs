using QrVision.Infra.Messaging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace QrVision.Worker.Consumers
{
    public abstract class RabbitMqConsumer(RabbitMqConnectionManager connectionManager)
        : BackgroundService
    {
        protected readonly RabbitMqConnectionManager _connectionManager = connectionManager;
        protected IChannel Channel { get; private set; }
        protected IConnection Connection { get; private set; }

        protected virtual async Task ConnectAsync(string queue, CancellationToken cancellationToken = default)
        {
            Connection ??= await _connectionManager.GetConnectionAsync(cancellationToken);
            Channel ??= await _connectionManager.GetChannelAsync(cancellationToken);

            var deadLetterQueueName = $"{queue}_error";
            var arguments = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", string.Empty }, // use default exchange
                { "x-dead-letter-routing-key", deadLetterQueueName }
            };

            await Channel.QueueDeclareAsync(queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
            arguments: arguments,
                cancellationToken: cancellationToken);
            await Channel.QueueDeclareAsync(deadLetterQueueName,
                durable: true,
                exclusive: false,
            autoDelete: false,
                cancellationToken: cancellationToken);
        }

        protected abstract Task ProcessMessageAsync(object sender, BasicDeliverEventArgs ea);
    }
}
