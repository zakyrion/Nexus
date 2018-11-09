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