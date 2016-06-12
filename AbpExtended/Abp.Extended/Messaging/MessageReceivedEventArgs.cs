using System;

namespace Abp.Messaging
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(IMessageArgs messageArgs)
        {
            MessageArgs = messageArgs;
        }

        public IMessageArgs MessageArgs { get; set; }
    }
}
