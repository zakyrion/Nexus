namespace Nexus.Core.Channels
{
    public interface IChannel
    {
        void Send(object param);
    }
}