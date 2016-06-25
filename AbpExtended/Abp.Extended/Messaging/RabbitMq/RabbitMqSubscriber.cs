using Abp.Dependency;
using Castle.Core.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Abp.Messaging.RabbitMq
{
    public abstract class RabbitMqSubscriber : DisposableObject, IMessageSubscriber
    {       
        private readonly RabbitMqCfg config;
        private readonly IConnection connection;
        private readonly IModel channel;
        private bool disposed;
        private readonly IIocResolver IocResolver;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;        

        public RabbitMqSubscriber(RabbitMqCfg config, IIocResolver iocResolver)
        {
            this.config = config;
            IocResolver = iocResolver;
            var factory = new ConnectionFactory() { Uri = config.Uri };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }

        public void Subscribe()
        {
            //declare a exchange
            channel.ExchangeDeclare(exchange: config.ExchangeName, type: "fanout", durable: true);

            //declare a queue
            channel.QueueDeclare(queue: config.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            //bind queue to exchange
            channel.QueueBind(queue: config.QueueName, exchange: config.ExchangeName, routingKey: config.RoutingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += ConsumerOnReceived;
            channel.BasicConsume(queue: config.QueueName, noAck: true, consumer: consumer);            
        }

        private void ConsumerOnReceived(object sender, BasicDeliverEventArgs e)
        {
            var json = Encoding.UTF8.GetString(e.Body);
            var message = JsonConvert.DeserializeObject<MessageArgs>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            var msgType = Type.GetType(message.AssemblyQualifiedName);

            ILogger logger = null;
            if (IocResolver.IsRegistered<ILogger>())
            {
                logger = IocResolver.Resolve<ILogger>();
            }

            using (var msgArgs = IocResolver.ResolveAsDisposable(msgType))
            {
                try
                {
                    var msgExecuteMethod = msgArgs.Object.GetType().GetMethod("Execute");
                    var argsType = msgExecuteMethod.GetParameters()[0].ParameterType;
                    var args = JsonConvert.DeserializeObject(message.Content, argsType);

                    msgExecuteMethod.Invoke(msgArgs.Object, new[] { args });

                    OnMessageReceived(new MessageReceivedEventArgs(message));
                    channel.BasicAck(e.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    if (logger != null)
                    {
                        logger.Warn(ex.Message, ex);
                    }
                }                
            }
        }

        private void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
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
