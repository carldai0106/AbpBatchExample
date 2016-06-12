using System;

namespace Abp.Messaging
{   
    public interface IMessagePublisher : IDisposable
    {
        void Publish<TMessage, TArgs>(TArgs args) where TMessage : IMessageExecutor<TArgs>;           
    }
}
