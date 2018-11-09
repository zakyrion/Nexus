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
using System.Collections.Concurrent;
using Nexus.ActionContainers;
using Nexus.Unity;
using UnityEngine;

namespace Nexus.ThreadSchedulers
{
    public class UnityThreadScheduler : IScheduler, IUpdated
    {
        private readonly int _maxActionsPerUpdate;
        private readonly ConcurrentQueue<IActionContainer> _actions = new ConcurrentQueue<IActionContainer>();

        public UnityThreadScheduler(int maxActionsPerUpdate = 0)
        {
            _maxActionsPerUpdate = maxActionsPerUpdate;
            UpdateService.Add(this);
        }

        public void Invoke(Action action)
        {
            _actions.Enqueue(new SimpleActionContainer(action));
        }

        public void Invoke<T>(Action<T> action, T param)
        {
            _actions.Enqueue(new ThreadActionContainer<T>(action, param));
        }

        public void Quit()
        {
            UpdateService.Remove(this);
        }

        public void Update()
        {
            var count = _maxActionsPerUpdate == 0 ? _actions.Count : Mathf.Min(_maxActionsPerUpdate, _actions.Count);
            for (var i = 0; i < count; i++)
            {
                IActionContainer actionContainer;
                if (_actions.TryDequeue(out actionContainer))
                    actionContainer.Invoke();
            }
        }
    }
}