using System;
using System.Collections.Generic;
using Nexus.Core.Channels;
using Nexus.ThreadSchedulers;

namespace Nexus.Core
{
    public class Nexus
    {
        private readonly IScheduler _scheduler;

        private readonly Dictionary<Adress, IChannel> _channels = new Dictionary<Adress, IChannel>();
        private readonly Dictionary<Adress, IListener> _listeners = new Dictionary<Adress, IListener>();
        private readonly Dictionary<IListener, List<Adress>> _adressesCache = new Dictionary<IListener, List<Adress>>();

        public Nexus(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public void Quit()
        {
            foreach (var listener in _listeners.Values)
                listener.Quit();

            _scheduler.Quit();
        }

        #region Listeners

        public void AddListener(Adress adress, IListener listener)
        {
            _scheduler.Invoke(AddListenerHandler, new Tuple<Adress, IListener>(adress, listener));
        }

        private void AddListenerHandler(Tuple<Adress, IListener> data)
        {
            var channel = new Channel(data.Item2);
            _channels.Add(data.Item1, channel);
            _listeners.Add(data.Item1, data.Item2);
            List<Adress> adresses;

            if (!_adressesCache.TryGetValue(data.Item2, out adresses))
            {
                adresses = new List<Adress>();
                _adressesCache.Add(data.Item2, adresses);
            }

            adresses.Add(data.Item1);
            data.Item2.OnAdd(this, channel);
        }

        public void RemoveListener(Adress adress)
        {
            _scheduler.Invoke(RemoveListenerHandler, adress);
        }

        private void RemoveListenerHandler(Adress adress)
        {
            var listener = _listeners[adress];
            _listeners.Remove(adress);

            _adressesCache[listener].Remove(adress);
            if (_adressesCache[listener].Count == 0)
            {
                _adressesCache.Remove(listener);
                listener.Quit();
            }
            else
            {
                listener.ChangeChannel(_channels[_adressesCache[listener][0]]);
            }

            _channels.Remove(adress);
        }

        public void RemoveListener(IListener listener)
        {
            _scheduler.Invoke(RemoveListenerHandler, listener);
        }

        private void RemoveListenerHandler(IListener listener)
        {
            var adresses = _adressesCache[listener];
            _adressesCache.Remove(listener);

            foreach (var adress in adresses)
            {
                _listeners.Remove(adress);
                _channels.Remove(adress);
            }

            listener.Quit();
        }

        #endregion

        public void GetChannel(Action<IChannel> callback, Adress adress)
        {
            _scheduler.Invoke(GetChannelHandler, new Tuple<Action<IChannel>, Adress>(callback, adress));
        }

        private void GetChannelHandler(Tuple<Action<IChannel>, Adress> data)
        {
            var channel = _channels[data.Item2];
            data.Item1(channel);
        }

        public void GetChannels(Action<IChannel> callback, params Adress[] adresses)
        {
            _scheduler.Invoke(GetChannelHandler, new Tuple<Action<IChannel>, Adress[]>(callback, adresses));
        }

        private void GetChannelHandler(Tuple<Action<IChannel>, Adress[]> data)
        {
            var channels = new List<IChannel>();
            foreach (var adress in data.Item2)
                channels.Add(_channels[adress]);
            data.Item1(new MultiChannel(channels));
        }
    }
}