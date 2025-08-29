using QrVision.Domain.Interfaces.Messaging;
using QrVision.Domain.Serialization;
using RabbitMQ.Client;
using System.Text;

namespace QrVision.Infra.Messaging
{
    public class RabbitMqMessagingProducer(RabbitMqConnectionManager rabbitMqConnectionManager) : IMessagingProducer, IAsyncDisposable
    {
        private IConnection _connection { get; set; }
        private IChannel _channel { get; set; }

        public async Task SendAsync<TMessage>(TMessage message, string queue)
        {
            _connection ??= await rabbitMqConnectionManager.GetConnectionAsync();
            _channel ??= await rabbitMqConnectionManager.GetChannelAsync();

            var deadLetterQueueName = $"{queue}_error";
            var arguments = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", string.Empty }, // use default exchange
                { "x-dead-letter-routing-key", deadLetterQueueName }
            };

            await _channel.QueueDeclareAsync(queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: arguments);
            await _channel.QueueDeclareAsync(deadLetterQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var serializedMessage = JsonSerializerHelper.Serialize(message);
            var messageBody = Encoding.UTF8.GetBytes(serializedMessage);
            var properties = new BasicProperties { Persistent = true };

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queue,
                true,
                basicProperties: properties,
                body: messageBody);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();

                _channel = null;
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();

                _connection = null;
            }

            GC.SuppressFinalize(this);
        }
    }

}
