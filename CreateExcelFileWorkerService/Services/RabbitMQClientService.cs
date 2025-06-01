using MDNET.Identity.RabbitMQ.Web.Confugration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateExcelFileWorkerService.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private ILogger<RabbitMQClientService> _logger;
        private IConnection _connection;
        private IModel _channel;
        
        
        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }
       
        public IModel Connect()
        {
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }
            else
            {
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
              //  _channel.ExchangeDeclare(exchange: _confugration.ExchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
               // _channel.QueueDeclare(queue: _confugration.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
             //   _channel.QueueBind(exchange: _confugration.ExchangeName, queue: _confugration.QueueName, routingKey: _confugration.RoutingKey);
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
