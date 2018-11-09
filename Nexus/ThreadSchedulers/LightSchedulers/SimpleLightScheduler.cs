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