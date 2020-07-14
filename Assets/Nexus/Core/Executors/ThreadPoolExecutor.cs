using System;
using System.Threading.Tasks;

public sealed class ThreadPoolExecutor : IExecutor
{
	public async Task<T> Run<T>(Func<Task<T>> action)
	{
		return await Task.Run(action);
	}

	public void Dispose()
	{
		
	}
}