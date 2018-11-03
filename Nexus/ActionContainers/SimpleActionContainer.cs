using System;

namespace Nexus.ActionContainers
{
    public class SimpleActionContainer : IActionContainer
    {
        private readonly Action _action;

        public SimpleActionContainer(Action action)
        {
            _action = action;
        }

        public void Invoke(object param = null)
        {
            _action?.Invoke();
        }
    }
}