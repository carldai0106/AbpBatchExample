namespace Abp.Net.Mail.RabbitMQ
{
    public interface IRabbitMQEmailPublisher
    {
        void Publish(string to, string subject, string body);
    }
}
