using System;

namespace Abp.Messaging
{    
    public interface IMessageSubscriber : IDisposable
    {        
        void Subscribe();
        
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
