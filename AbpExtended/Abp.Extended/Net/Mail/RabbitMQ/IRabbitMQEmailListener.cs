namespace Abp.Net.Mail.RabbitMQ
{
    public interface IRabbitMQEmailListener
    {
        void Start();

        void Stop();
    }
}
