using Nexus.Core.Channels;

namespace Nexus.Core
{
    public interface IListener
    {
        void Receive(object param);
        void OnAdd(Nexus nexus, IChannel toThis);
        void ChangeChannel(IChannel channel);
        void Quit();
    }
}