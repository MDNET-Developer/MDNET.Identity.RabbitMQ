using MDNET.Identity.RabbitMQ.Web.Confugration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

namespace MDNET.Identity.RabbitMQ.Web.Services
{
    public class RabbitMQPublisher
    {
        private RabbitMQClientService _rabbitmqClientService;
        private RabbitMQConfugration _rabbitmqConfugration;
        private ILogger<RabbitMQPublisher> _logger;

        public RabbitMQPublisher(RabbitMQClientService rabbitmqClientService, IOptions<RabbitMQConfugration> rabbitmqConfugration, ILogger<RabbitMQPublisher> logger)
        {
            _rabbitmqClientService = rabbitmqClientService;
            _rabbitmqConfugration = rabbitmqConfugration.Value;
            _logger = logger;
        }

        public void Publish(CreateFileMessage message)
        {

            var bodyString = JsonSerializer.Serialize(message);
            var bodyByte = Encoding.UTF8.GetBytes(bodyString);

            var channel = _rabbitmqClientService.Connect();
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(
          exchange: _rabbitmqConfugration.ExchangeName,
          routingKey: _rabbitmqConfugration.RoutingKey,
          basicProperties: properties,
          body: bodyByte);
            /*
            Davamlılıq (Persistence) ilə bağlı önəmli məlumat:
               RabbitMQ-da bir mesajın davamlı (persistent) olması, onun məlumatların daimi olaraq saxlanılmasını təmin edir. Bu, xüsusən kritik mesajların itilməməsi üçün vacibdir. Lakin davamlılıq əlavə disk əməliyyatlarına səbəb olur və performansı bir qədər azalda bilər.

            Davamlı mesajlar: Diskdə saxlanılır və server yenidən başladıqda belə mesajlar itmir.
            Yaddaşda saxlanılan mesajlar: Yalnız RabbitMQ serverində saxlanılır, server yenidən başladığında itə bilər.
            */
        }
    }
}
