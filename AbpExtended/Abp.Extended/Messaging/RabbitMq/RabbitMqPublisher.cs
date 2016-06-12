using Abp.Dependency;
using Abp.Json;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Messaging.RabbitMq
{
    public abstract class RabbitMqPublisher : DisposableObject, IMessagePublisher, ITransientDependency
    {        
        private readonly RabbitMqCfg config;
        private readonly IConnection connection;
        private readonly IModel channel;
        private bool disposed;

        public RabbitMqPublisher(RabbitMqCfg config)
        {
            this.config = config;
            var factory = new ConnectionFactory() { Uri = config.Uri };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Publish<TMessage, TArgs>(TArgs args) 
            where TMessage : IMessageExecutor<TArgs>            
        {            
            var msgArgs = new MessageArgs
            {
                AssemblyQualifiedName = typeof(TMessage).AssemblyQualifiedName,
                Content = args.ToJsonString()
            };

            var json = JsonConvert.SerializeObject(msgArgs, 
                Formatting.Indented, 
                new JsonSerializerSettings {
                    TypeNameHandling = TypeNameHandling.All
                });

            var msg = Encoding.UTF8.GetBytes(json);

            var _properties = channel.CreateBasicProperties();
            _properties.DeliveryMode = 2;

            channel.BasicPublish(
                exchange: config.ExchangeName,
                routingKey: config.RoutingKey,
                basicProperties: _properties,
                body: msg);            
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    channel.Close(200, "Goodbye");
                    channel.Dispose();

                    connection.Close();
                    connection.Dispose();
                    disposed = true;
                }
            }
        }
    }
}
