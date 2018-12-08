﻿//   Copyright Nexus Kharsun Sergey
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
using Nexus.ActionContainers;
using Nexus.Messages;
using UnityEngine;

namespace Nexus.Actors
{
    public class BindManager
    {
        protected Dictionary<Type, List<IActionContainer>> Actions = new Dictionary<Type, List<IActionContainer>>();
        protected Action<Exception, dynamic> ExceptionHandler;

        private readonly Dictionary<int, IActionContainer> _requests = new Dictionary<int, IActionContainer>();
        private int _currentId;

        public void SetExceptionHandler(Action<Exception, dynamic> exceptionHandler)
        {
            ExceptionHandler = exceptionHandler;
        }

        public void BindAction<T>(Action action)
        {
            var containers = GetContainers<T>();
            containers.Add(new SimpleActionContainer(action));
        }

        private List<IActionContainer> GetContainers<T>()
        {
            var type = typeof(T);
            List<IActionContainer> containers = null;

            if (!Actions.TryGetValue(type, out containers))
            {
                containers = new List<IActionContainer>();
                Actions.Add(type, containers);
            }

            return containers;
        }

        public void BindAction<T>(Action<T> action)
        {
            var containers = GetContainers<T>();
            containers.Add(new ActionContainer<T>(action));
        }

        public void BindReturnAction<T>(Action<IMessage, T> action)
        {
            var containers = GetContainers<T>();
            containers.Add(new ReturnActionContainer<T>(action));
        }

        public int BindRequest<T>(Action<T> callback)
        {
            var id = _currentId;
            _currentId++;

            _requests.Add(id, new ActionContainer<T>(callback));
            return id;
        }

        public void InvokeCallback(int id, object param)
        {
            _requests[id].Invoke(param);
            _requests.Remove(id);
        }

        public void Invoke(object param)
        {
            var type = param.GetType();

            List<IActionContainer> containers;

            try
            {
                if (Actions.TryGetValue(type, out containers))
                {
                    foreach (var container in containers)
                        container.Invoke(param);
                }
                else
                {
                    var message = param as IMessage;
                    if (message != null)
                    {
                        if (message.IsRequest)
                        {
                            type = message.Data.GetType();
                            if (Actions.TryGetValue(type, out containers))
                                foreach (var container in containers)
                                    container.Invoke(param);
                        }
                        else
                        {
                            InvokeCallback(message.Id, message.Data);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Exception: " + e);
                ExceptionHandler?.Invoke(e, param);
            }
        }
    }
}