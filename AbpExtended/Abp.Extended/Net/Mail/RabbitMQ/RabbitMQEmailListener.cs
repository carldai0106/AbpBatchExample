using System.Net.Mail;
using System.Text;
using Abp.Dependency;
using Abp.Net.Mail.Smtp;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Abp.Net.Mail.RabbitMQ
{
    public class RabbitMQEmailListener : IRabbitMQEmailListener, ISingletonDependency
    {
        private IConnection _connection;
        private IModel _channel;

        private readonly ISmtpEmailSender _sender;
        private readonly IConnectionFactory _factory;

        public RabbitMQEmailListener(ISmtpEmailSender sender, IConnectionFactory factory)
        {
            _sender = sender;
            _factory = factory;
        }

        public void Start()
        {
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(
                exchange: AbpExtendedConsts.ExchangeName,
                type: "fanout",
                durable: true
                );

            var queueName = _channel.QueueDeclare().QueueName;

            _channel.QueueBind(
                queue: queueName,
                exchange: AbpExtendedConsts.ExchangeName,
                routingKey: "");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ConsumerOnReceived;

            _channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
        }

        public void Stop()
        {
            _channel.Close(200, "Goodbye");
            _connection.Close();
        }

        private void ConsumerOnReceived(object sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body);
            var body = JsonConvert.DeserializeObject<EmailMessage>(message);
            var email = new MailMessage(body.FromAddress, body.To, body.Subject, body.Body)
            {
                IsBodyHtml = true
            };

            _sender.Send(email);
        } 
    }
}
