using System;
using Nexus.Messages;

namespace Nexus.ActionContainers
{
    public class ReturnActionContainer<T> : IActionContainer
    {
        private readonly Action<IMessage, T> _action;

        public ReturnActionContainer(Action<IMessage, T> action)
        {
            _action = action;
        }

        public void Invoke(object param = null)
        {
            IMessage message = (IMessage)param;
            _action?.Invoke(message, (T)message.Data);
        }
    }
}