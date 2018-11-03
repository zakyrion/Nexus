using System;

namespace Nexus.ActionContainers
{
    internal sealed class ThreadActionContainer<T> : IActionContainer
    {
        private readonly T _param;
        private readonly Action<T> _action;

        public void Invoke(object param = null)
        {
            _action?.Invoke(_param);
        }

        public ThreadActionContainer(Action<T> action, T param)
        {
            _param = param;
            _action = action;
        }
    }
}