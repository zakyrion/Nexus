using System;
using System.Collections.Generic;
using Nexus.ActionContainers;
using Nexus.Messages;

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
        }

        public void Invoke(object param)
        {
            var type = param.GetType();
            Invoke(type, param);
        }

        protected void Invoke(Type type, object param)
        {
            List<IActionContainer> containers = null;

            try
            {
                if (Actions.TryGetValue(type, out containers))
                    foreach (var container in containers)
                        container.Invoke(param);
                else if (type == typeof(Message))
                {
                    var message = (Message)param;
                    if (message.IsRequest)
                    {
                        type = message.Data.GetType();
                        if (Actions.TryGetValue(type, out containers))
                            foreach (var container in containers)
                                container.Invoke(param);
                    }
                    else
                        InvokeCallback(message.Id, message.Data);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler?.Invoke(e, param);
            }
        }
    }
}