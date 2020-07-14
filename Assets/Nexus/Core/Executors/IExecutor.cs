using System;
using System.Threading.Tasks;

public interface IExecutor : IDisposable
{
	Task<R> Run<R>(Func<Task<R>> action);
}