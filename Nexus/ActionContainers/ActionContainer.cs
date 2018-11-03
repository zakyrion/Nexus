using System;

namespace Nexus.ActionContainers
{
    public class ActionContainer<T> : IActionContainer
    {
        private Action<T> _action;

        public ActionContainer(Action<T> action)
        {
            _action = action;
        }

        public void Invoke(object param = null)
        {
            _action?.Invoke((T)param);
        }
    }
}