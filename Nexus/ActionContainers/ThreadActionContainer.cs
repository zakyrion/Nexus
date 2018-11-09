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

namespace Nexus.ActionContainers
{
    internal sealed class ThreadActionContainer<T> : IActionContainer
    {
        private readonly T _param;
        private readonly Action<T> _action;

        public void Invoke(object param = null)
        {
            _action?.Invoke(_param);
        }

        public ThreadActionContainer(Action<T> action, T param)
        {
            _param = param;
            _action = action;
        }
    }
}