using Nexus.ThreadSchedulers.LightSchedulers;

namespace Nexus.ThreadSchedulers
{
    public interface IMessageScheduler
    {
        void Add(ILightScheduler scheduler);
        void Remove(ILightScheduler scheduler);
        void Quit();
    }
}