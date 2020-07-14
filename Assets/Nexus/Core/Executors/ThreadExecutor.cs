using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ThreadExecutor : Executor
{
	private Thread _thread;

	ManualResetEvent _resetEvent = new ManualResetEvent(false);
	
	public ThreadExecutor()
	{
		_thread = new Thread(Loop);
		_thread.Start();
	}

	private int _test;

	public override Task<R> Run<R>(Func<Task<R>> action)
	{
		var result = base.Run(action);
		_resetEvent.Set();
		
		return result;
	}

	void Loop()
	{
		while (true)
		{
			while (Tasks.TryDequeue(out var task))
				task.Invoke();

			_resetEvent.WaitOne();
			_resetEvent.Reset();
		}	
	}

	public override void Dispose()
	{
		_thread.Abort();
	}
}