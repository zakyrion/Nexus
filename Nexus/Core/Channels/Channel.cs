namespace Nexus.Core.Channels
{
    public class Channel : IChannel
    {
        protected IListener Listener;

        public Channel(IListener listener)
        {
            Listener = listener;
        }

        public void Send(object param)
        {
            Listener.Receive(param);
        }
    }
}