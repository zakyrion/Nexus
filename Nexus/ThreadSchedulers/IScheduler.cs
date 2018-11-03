using System;

namespace Nexus.ThreadSchedulers
{
    public interface IScheduler
    {
        void Invoke(Action action);
        void Invoke<T>(Action<T> action, T param);
        void Quit();
    }
}