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