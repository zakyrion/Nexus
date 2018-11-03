using System;
using System.Collections.Generic;
using System.Threading;
using Nexus.ThreadSchedulers.LightSchedulers;
using UnityEngine;

namespace Nexus.ThreadSchedulers
{
    public class ThreadMessageScheduler : IMessageScheduler
    {
        protected volatile List<ILightScheduler> Schedulers = new List<ILightScheduler>();
        private readonly Thread _thread;
        private readonly ManualResetEvent _resetEvent = new ManualResetEvent(true);

        public ThreadMessageScheduler()
        {
            _thread = new Thread(WorkCicle);
            _thread.Start();
        }

        public void Add(ILightScheduler scheduler)
        {
            Schedulers.Add(scheduler);
            scheduler.OnInvoke += TryRunCicle;
        }

        private void TryRunCicle()
        {
            _resetEvent.Set();
        }

        public void Remove(ILightScheduler scheduler)
        {
            scheduler.OnInvoke -= TryRunCicle;
            Schedulers.Remove(scheduler);
        }

        public void Quit()
        {
            _thread.Abort();
        }

        private void WorkCicle()
        {
            while (true)
            {
                var executed = 0;

                try
                {
                    for (var i = 0; i < Schedulers.Count; i++)
                    {
                        executed += Schedulers[i].Actions;
                        Schedulers[i].Execute();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"Catch exception: {e}");
                    //TODO add logger
                }

                if (executed == 0)
                {
                    _resetEvent.WaitOne();
                    _resetEvent.Reset();
                }
            }
        }
    }
}