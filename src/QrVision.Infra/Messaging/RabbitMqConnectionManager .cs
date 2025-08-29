using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace QrVision.Infra.Messaging
{
    public class RabbitMqConnectionManager(IConfiguration configuration) : IAsyncDisposable
    {
        private readonly IConfiguration _configuration = configuration;
        private IConnection _connection;
        private IChannel _channel;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);

        public async Task<IConnection> GetConnectionAsync(CancellationToken stoppingToken = default)
        {
            if (_connection != null && _connection.IsOpen) return _connection;

            await _connectionLock.WaitAsync(stoppingToken);

            try
            {
                if (_connection != null && _connection.IsOpen) return _connection;

                var connectionFactory = new ConnectionFactory
                {
                    Port = int.Parse(_configuration["RABBITMQ_PORT"]),
                    HostName = _configuration["RABBITMQ_HOSTNAME"],
                    VirtualHost = _configuration["RABBITMQ_VIRTUALHOST"],
                    UserName = _configuration["RABBITMQ_USERNAME"],
                    Password = _configuration["RABBITMQ_PASSWORD"],
                };

                _connection = await connectionFactory.CreateConnectionAsync(stoppingToken);

                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken = default)
        {
            if (_channel != null && _channel.IsOpen) return _channel;

            await _connectionLock.WaitAsync(cancellationToken);

            try
            {
                if (_channel != null && _channel.IsOpen) return _channel;

                var connection = await GetConnectionAsync(cancellationToken);

                _channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

                return _channel;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }

            _connectionLock.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
