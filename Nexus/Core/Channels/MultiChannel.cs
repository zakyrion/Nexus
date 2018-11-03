using System.Collections.Generic;

namespace Nexus.Core.Channels
{
    public class MultiChannel : IChannel
    {
        protected List<IChannel> Listeners;

        public MultiChannel(List<IChannel> listeners)
        {
            Listeners = listeners;
        }

        public void Send(object param)
        {
            foreach (var channel in Listeners) channel.Send(param);
        }
    }
}