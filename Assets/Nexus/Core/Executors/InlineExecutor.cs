using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public abstract class Executor : IExecutor
{
	protected readonly ConcurrentQueue<IProxyTask> Tasks = new ConcurrentQueue<IProxyTask>();

	public virtual void Dispose()
	{
	}

	public virtual Task<R> Run<R>(Func<Task<R>> action)
	{
		var taskCompletion = new TaskCompletionSource<R>();
		Tasks.Enqueue(new ProxyTask<R>(taskCompletion, action));
		return taskCompletion.Task;
	}

	protected interface IProxyTask
	{
		Task<bool> Invoke();
	}

	protected class ProxyTask<T> : IProxyTask
	{
		private readonly TaskCompletionSource<T> _taskCompletion;
		private readonly Func<Task<T>> _func;

		public ProxyTask(TaskCompletionSource<T> completionSource, Func<Task<T>> func)
		{
			_taskCompletion = completionSource;
			_func = func;
		}

		public async Task<bool> Invoke()
		{
			var result = await _func();
			_taskCompletion.SetResult(result);

			return true;
		}
	}
}