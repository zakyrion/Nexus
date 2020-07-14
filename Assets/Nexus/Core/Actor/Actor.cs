using System;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Core;
using UnityEngine;


/*
 * актор полностью закрыт к внешнему миру
 * у актора есть вложенный класс через который и происходит взаимодействие мира с актором
 * класс Invoker добавляет вызовы всех методов в очередь выполнения, а так же передает параметры.
 * Больше нет никаких сообщений и тд, только Invoker и запросы к нему
 */
public class Actor<T> : IInvokable where T : IInvoker
{
	protected Node.Invoker Parent;
	protected Address Address;
	protected IExecutor Executor;
	protected readonly BindManager BindManager;

	protected Actor(IExecutor executor)
	{
		BindManager = new BindManager(executor);
		Executor = executor;
	}

	public virtual void OnAdd(Address address, Node.Invoker parent)
	{
		Debug.Log($"On Add: {Thread.CurrentThread.ManagedThreadId}");

		Executor.Run(async () =>
		{
			Debug.Log($"Call lambda: {Thread.CurrentThread.ManagedThreadId}");
			Address = address;
			Parent = parent;
			return true;
		});
	}

	private int _count;

	#region Invoke

	public virtual IInvoker InvokerCurrent { get; protected set; }

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
}