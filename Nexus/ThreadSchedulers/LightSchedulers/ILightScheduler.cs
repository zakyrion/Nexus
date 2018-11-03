using System;

namespace Nexus.ThreadSchedulers.LightSchedulers
{
    public interface ILightScheduler
    {
        event Action OnInvoke;
        int Actions { get; }
        void Invoke(object param);
        void Execute();
        void Execute(int count);
    }
}