using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BindManager : IDisposable
{
	private readonly IExecutor _executor;
	private readonly object _lock = new object();
	private volatile int _signal = 0;
	
	private Queue<IActionContainer> _current = new Queue<IActionContainer>();
	private Queue<IActionContainer> _wait = new Queue<IActionContainer>();

	public BindManager(IExecutor executor)
	{
		_executor = executor;
	}

	#region Invoke

	public void Invoke(Action action)
	{
		lock (_lock)
		{
			_wait.Enqueue(new ActionContainer(action));
			RunLoop();
		}
	}

	public void Invoke<T>(Action<T> action, T param1)
	{
		lock (_lock)
		{
			_wait.Enqueue(new ActionContainer<T>(action, param1));
			RunLoop();
		}
	}

	public void Invoke<T, T1>(Action<T, T1> action, T param1, T1 param2)
	{
		lock (_lock)
		{
			_wait.Enqueue(new ActionContainer<T, T1>(action, param1, param2));
			RunLoop();
		}
	}

	public void Invoke<T, T1, T2>(Action<T, T1, T2> action, T param1, T1 param2, T2 param3)
	{
		lock (_lock)
		{
			_wait.Enqueue(new ActionContainer<T, T1, T2>(action, param1, param2, param3));
			RunLoop();
		}
	}

	#region Functions

	public Task<R> Invoke<R>(Func<R> func)
	{
		var source = new TaskCompletionSource<R>();
		lock (_lock)
		{
			_wait.Enqueue(new FunctionContainer<R>(func, source));
			RunLoop();
		}

		return source.Task;
	}

	public Task<R> Invoke<R, T>(Func<T, R> func, T param)
	{
		var source = new TaskCompletionSource<R>();
		lock (_lock)
		{
			_wait.Enqueue(new FunctionContainer<R, T>(func, source, param));
			RunLoop();
		}

		return source.Task;
	}

	public Task<R> Invoke<R, T, T1>(Func<T, T1, R> func, T param, T1 param1)
	{
		var source = new TaskCompletionSource<R>();
		lock (_lock)
		{
			_wait.Enqueue(new FunctionContainer<R, T, T1>(func, source, param, param1));
			RunLoop();
		}

		return source.Task;
	}

	public Task<R> Invoke<R, T, T1, T2>(Func<T, T1, R> func, T param, T1 param1, T2 param2)
	{
		var source = new TaskCompletionSource<R>();
		lock (_lock)
		{
			_wait.Enqueue(new FunctionContainer<R, T, T1>(func, source, param, param1));
			RunLoop();
		}

		return source.Task;
	}

	#endregion

	protected void RunLoop()
	{
		if (_signal != 0)
			return;

		_signal++;
		_executor.Run(Execute);
	}

	#endregion


	private async Task<bool> Execute()
	{
		try
		{
			lock (_lock)
			{
				var temp = _wait;
				--_signal;
				_wait = _current;
				_current = temp;
			}

			while (_current.Count + _wait.Count != 0)
			{
				while (_current.Count != 0)
				{
					var actionContainer = _current.Dequeue();

					actionContainer.Invoke();
				}

				/*Debug.Log(
					$"Before lock. Current: {_current.Count} wait: {_wait.Count} timestamp: {Environment.TickCount}");*/

				lock (_lock)
				{
					var temp = _wait;
					_wait = _current;
					_current = temp;
				}
			}

		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}

		return true;
	}

	public void Dispose()
	{
		_executor?.Dispose();
	}
}

/*
 * 1 - создать свой скедулер, все таски создавать через фабрику, пропихивать свой контекст и пытаться понять когда идет переключение.
 *
 *
 * 
 * 2 - обрабатывать только запросы и в ручном режиме переводить актора в обработку "второго контура" который будет состоять из ответов на одни только запросы. 
*/