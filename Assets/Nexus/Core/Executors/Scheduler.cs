using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Scheduler : TaskScheduler
{
	private readonly SynchronizationContext _context;

	internal Scheduler(SynchronizationContext context)
	{
		this._context = context;
	}

	[SecurityCritical]
	protected override void QueueTask(Task task)
	{
		Debug.LogError($"QueueTask: {task}");
		_context.Post(PostCallback, task);
	}

	[SecurityCritical]
	protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
	{
		Debug.LogError($"TryExecuteTaskInline: {SynchronizationContext.Current == this._context}");
		if (SynchronizationContext.Current == this._context)
			return TryExecuteTask(task);

		return false;
	}

	[SecurityCritical]
	protected override IEnumerable<Task> GetScheduledTasks()
	{
		return null;
	}

	public override int MaximumConcurrencyLevel => 1;

	private void PostCallback(object obj)
	{
		Debug.LogError($"PostCallback: {(Task) obj}");
		TryExecuteTask((Task) obj);
	}
}