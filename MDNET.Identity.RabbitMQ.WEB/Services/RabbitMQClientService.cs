using MDNET.Identity.RabbitMQ.Web.Confugration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MDNET.Identity.RabbitMQ.Web.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private ILogger<RabbitMQClientService> _logger;
        private RabbitMQConfugration _confugration;
        private IConnection _connection;
        private IModel _channel;


        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger, IOptions<RabbitMQConfugration> confugration)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _confugration = confugration.Value;
        }

        public IModel Connect()
        {
            if (_channel is { IsOpen:true})
            {
                return _channel;
            }
            else
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: _confugration.ExchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
                _channel.QueueDeclare(queue: _confugration.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                _channel.QueueBind(exchange: _confugration.ExchangeName, queue: _confugration.QueueName, routingKey: _confugration.RoutingKey);
                Console.ForegroundColor = ConsoleColor.Yellow;
                _logger.LogInformation($"RabbitMQ create connection - {DateTime.UtcNow}");
                Console.ForegroundColor = ConsoleColor.White;
                return _channel;
            }
           
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ down connection");
        }
    }
}
