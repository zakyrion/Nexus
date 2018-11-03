using System;

namespace Nexus.ThreadSchedulers
{
    public class SimpleScheduler : IScheduler
    {
        public void Invoke(Action action)
        {
            action();
        }

        public void Invoke<T>(Action<T> action, T param)
        {
            action(param);
        }

        public void Quit()
        {
        }
    }
}