namespace MDNET.Identity.RabbitMQ.Web.Confugration
{
    public class RabbitMQConfugration
    {
        public string? URI { get; set; }
        public string? ExchangeName { get; set; }
        public string? RoutingKey { get; set; }
        public string? QueueName { get; set; }
    }
}
