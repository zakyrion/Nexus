using System;
using System.Threading.Tasks;
using Nexus.Core;
using UnityEngine;

	public abstract class MonoActor<T> : MonoBehaviour, IInvokable where T : IInvoker
	{
		protected Node.Invoker Parent;
		protected IExecutor Executor;
		protected BindManager BindManager;

		protected virtual void Awake()
		{
			Executor = new UnityExecutor();
			BindManager = new BindManager(Executor);
			Init();
		}

		protected abstract void Init();

		public virtual void OnAdd(Address address, Node.Invoker parent)
		{
			Executor.Run(async () =>
			{
				if (address.IsEmpty())
					Address = address;
				
				Parent = parent;
				return true;
			});
		}

		private int _count;

		#region Invoke

		public abstract IInvoker InvokerCurrent { get; }

		public virtual R GetInvoker<R>() where R : IInvoker
		{
			return InvokerCurrent.Convert<R>();
		}
	
		public virtual T GetInvoker()
		{
			return InvokerCurrent.Convert<T>();
		}

		#region Actions

		protected void Invoke(Action action)
		{
			BindManager.Invoke(action);
		}
	
		protected void Invoke<T>(Action<T> action, T param1)
		{
			BindManager.Invoke(action, param1);
		}
	
		protected void Invoke<T,T1>(Action<T,T1> action, T param1, T1 param2)
		{
			BindManager.Invoke(action,param1,param2);
		}

		protected void Invoke<T, T1, T2>(Action<T, T1, T2> action, T param1, T1 param2, T2 param3)
		{
			BindManager.Invoke(action, param1, param2, param3);
		}

		#endregion


		#region Functions

		protected Task<R> Invoke<R>(Func<R> func)
		{
			return BindManager.Invoke(func);
		}
	
		protected Task<R> Invoke<R,T1>(Func<T1,R> func, T1 param)
		{
			return BindManager.Invoke(func, param);
		}
	
		protected Task<R> Invoke<R,T1,T2>(Func<T1,T2,R> func, T1 param1, T2 param2)
		{
			return BindManager.Invoke(func, param1, param2);
		}

		#endregion

		#endregion

		public void Dispose()
		{
			BindManager.Dispose();
		}

		public virtual Address Address { get; protected set; }
	}