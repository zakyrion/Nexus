using System.Collections.Generic;
using Nexus.ThreadSchedulers.LightSchedulers;
using Nexus.Unity;

namespace Nexus.ThreadSchedulers
{
    public class UnityMessageScheduler : IMessageScheduler, IUpdated
    {
        protected volatile List<ILightScheduler> Schedulers = new List<ILightScheduler>();
        private readonly int _maxActions = 1000;
        
        public UnityMessageScheduler(int maxActions = 1000)
        {
            _maxActions = maxActions;
            UpdateService.Add(this);
        }

        public void Add(ILightScheduler scheduler)
        {
            Schedulers.Add(scheduler);
        }

        public void Remove(ILightScheduler scheduler)
        {
            Schedulers.Remove(scheduler);
        }

        public void Quit()
        {
            UpdateService.Remove(this);
        }

        public void Update()
        {
            int maxAction = _maxActions;
            
            for (var i = 0; i < Schedulers.Count; i++)
            {
                Schedulers[i].Execute(maxAction);
                maxAction -= Schedulers[i].Actions;
                if (maxAction < 0)
                    break;
            }
        }
    }
}