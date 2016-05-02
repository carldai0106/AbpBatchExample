using System.Text;
using Abp.Dependency;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Abp.Net.Mail.RabbitMQ
{
    public class RabbitMQEmailPublisher : IRabbitMQEmailPublisher, ITransientDependency
    {
        private readonly IEmailSenderConfiguration _emailSenderConfiguration;
        private readonly IConnectionFactory _factory;
        public RabbitMQEmailPublisher(
            IEmailSenderConfiguration emailSenderConfiguration, IConnectionFactory factory)
        {
            _emailSenderConfiguration = emailSenderConfiguration;
            _factory = factory;
        }

        public void Publish(string to, string subject, string body)
        {
            var em = new EmailMessage
            {
                FromAddress = _emailSenderConfiguration.DefaultFromAddress,
                To = to,
                Subject = subject,
                Body = body
            };

            using (var connection = _factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(
                        exchange: AbpExtendedConsts.ExchangeName, 
                        type: "fanout", 
                        durable: true);

                    var json = JsonConvert.SerializeObject(em);
                    var msg = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(
                        exchange: AbpExtendedConsts.ExchangeName,
                        routingKey: "", 
                        basicProperties: null,
                        body: msg);
                }
            }
        }
    }
}
