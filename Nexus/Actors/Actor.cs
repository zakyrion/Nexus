﻿using System;
using System.Collections.Generic;
using Nexus.ActionContainers;
using Nexus.Core;
using Nexus.Core.Channels;
using Nexus.Messages;
using Nexus.ThreadSchedulers;
using Nexus.ThreadSchedulers.LightSchedulers;

namespace Nexus.Actors
{
    public class Actor : IListener
    {
        private readonly BindManager _bindManager = new BindManager();
        protected Core.Nexus Nexus;
        protected IMessageScheduler Scheduler;
        protected ILightScheduler LightScheduler;
        protected IChannel ToMe;
        private bool _isQuit;

        public Actor(IMessageScheduler scheduler)
        {
            Scheduler = scheduler;
            LightScheduler = new SimpleLightScheduler(_bindManager);
            Scheduler.Add(LightScheduler);
            Subscribe();
        }

        protected virtual void Subscribe()
        {
        }

        /// <summary>
        /// Receive message from other actor
        /// </summary>
        /// <param name="action">callback</param>
        /// <typeparam name="T">invoke message type</typeparam>
        protected void BindAction<T>(Action action)
        {
            _bindManager.BindAction<T>(action);
        }

        /// <summary>
        /// Receive message from other actor
        /// </summary>
        /// <param name="action">callback</param>
        /// <typeparam name="T">invoke message type</typeparam>
        protected void BindAction<T>(Action<T> action)
        {
            _bindManager.BindAction(action);
        }

        protected void BindReturnAction<T>(Action<IMessage, T> action)
        {
            _bindManager.BindReturnAction(action);
        }
        
        protected void SendMessage<T>(IChannel channel, T param)
        {
            channel.Send(param);
        }

        protected void SendMessage<T, R>(IChannel channel, T param, Action<R> callback)
        {
            var id = _bindManager.BindRequest(callback);
            var request = new Message(id, true, param, ToMe);
            channel.Send(request);
        }

        protected void Respounce<T>(IMessage message, T param)
        {
            message.IsRequest = false;
            message.Data = param;
            message.Respounce(message);
        }

        public void Receive(object param)
        {
            if (!_isQuit)
                LightScheduler.Invoke(param);
        }

        public virtual void OnAdd(Core.Nexus nexus, IChannel toThis)
        {
            ToMe = toThis;
            Nexus = nexus;
        }

        public void ChangeChannel(IChannel channel)
        {
            ToMe = channel;
        }

        public virtual void Quit()
        {
            if (!_isQuit)
            {
                _isQuit = true;
                Scheduler.Quit();
            }
        }
    }
}