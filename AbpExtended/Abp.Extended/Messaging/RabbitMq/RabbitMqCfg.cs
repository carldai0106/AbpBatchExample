namespace Abp.Messaging.RabbitMq
{
    public class RabbitMqCfg
    {
        public string Uri { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }
    }
}
