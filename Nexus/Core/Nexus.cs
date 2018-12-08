//   Copyright Nexus Kharsun Sergey
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

		public async Task<IChannel> GetChannel(Adress adress)
		{
			IChannel channel = null;
			var @event = new ManualResetEvent(false);

			Action<IChannel> callback = result =>
			{
				@event.Set();
				channel = result;
			};

			GetChannel(callback, adress);

			@event.WaitOne();
			@event.Reset();

			return channel;
		}

		public void GetChannel(Action<IChannel> callback, Adress adress)
		{
			_scheduler.Invoke(GetChannelHandler, new Tuple<Action<IChannel>, Adress>(callback, adress));
		}

		private void GetChannelHandler(Tuple<Action<IChannel>, Adress> data)
		{
			var channel = _channels[data.Item2];
			data.Item1(channel);
		}

		public async Task<IChannel> GetChannels(params Adress[] adresses)
		{
			IChannel channel = null;
			var @event = new ManualResetEvent(false);

			Action<IChannel> callback = result =>
			{
				@event.Set();
				channel = result;
			};

			GetChannels(callback, adresses);

			@event.WaitOne();
			@event.Reset();

			return channel;
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