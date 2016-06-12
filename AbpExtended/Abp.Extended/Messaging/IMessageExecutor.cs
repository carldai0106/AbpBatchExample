using System;

namespace Abp.Messaging
{
    public interface IMessageExecutor<TArgs>
    {
        Guid Id { get; set; }
        void Execute(TArgs args);
    }
}
