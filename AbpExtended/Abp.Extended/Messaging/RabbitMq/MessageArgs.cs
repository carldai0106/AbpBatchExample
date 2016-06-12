namespace Abp.Messaging.RabbitMq
{
    public class MessageArgs : IMessageArgs
    {
        public string AssemblyQualifiedName { get; set; }

        public string Content { get; set; }       
    }
}
