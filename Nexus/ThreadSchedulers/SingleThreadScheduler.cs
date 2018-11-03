using System;
using System.Collections.Concurrent;
using System.Threading;
using Nexus.ActionContainers;

namespace Nexus.ThreadSchedulers
{
    public class SingleThreadScheduler : IScheduler
    {
        private readonly Thread _thread;
        private readonly ConcurrentQueue<IActionContainer> _actions = new ConcurrentQueue<IActionContainer>();
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(true);

        public SingleThreadScheduler()
        {
            _thread = new Thread(WorkCicle);
            _thread.Start();
        }

        private void WorkCicle()
        {
            while (true)
            {
                try
                {
                    while (!_actions.IsEmpty)
                    {
                        IActionContainer actionContainer;
                        if (_actions.TryDequeue(out actionContainer))
                            actionContainer.Invoke();

                        if (_actions.IsEmpty)
                            Thread.SpinWait(5);
                    }
                }
                catch (Exception e)
                {
                    //TODO add logger
                }
                
                _resetEvent.Reset();
                _resetEvent.WaitOne();
            }
        }

        public void Invoke(Action action)
        {
            _actions.Enqueue(new SimpleActionContainer(action));
            _resetEvent.Set();
        }

        public void Invoke<T>(Action<T> action, T param)
        {
            _actions.Enqueue(new ThreadActionContainer<T>(action, param));
            _resetEvent.Set();
        }

        public void Quit()
        {
            _thread.Abort();
        }
    }
}