using System;
using System.Collections.Concurrent;
using Nexus.Actors;

namespace Nexus.ThreadSchedulers.LightSchedulers
{
    public class SimpleLightScheduler : ILightScheduler
    {
        protected BindManager Manager;
        protected ConcurrentQueue<object> Params = new ConcurrentQueue<object>();

        private bool _isExecute;

        public SimpleLightScheduler(BindManager manager)
        {
            Manager = manager;
        }

        public event Action OnInvoke;

        public int Actions => Params.Count;

        public void Invoke(object param)
        {
            Params.Enqueue(param);

            if (!_isExecute)
                OnInvoke?.Invoke();
        }

        public void Execute()
        {
            _isExecute = true;

            try
            {
                object param;
                while (Params.TryDequeue(out param))
                    Manager.Invoke(param);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _isExecute = false;
            }
        }

        public void Execute(int count)
        {
            _isExecute = true;

            try
            {
                object param;
                while (Params.TryDequeue(out param) && --count > 0)
                    Manager.Invoke(param);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _isExecute = false;
            }
        }
    }
}