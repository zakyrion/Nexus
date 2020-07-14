using System.Threading.Tasks;

public class UnityExecutor : Executor, IUpdateble
{
	private readonly int _countOfExecution;

	public UnityExecutor(int countOfExecution = 1000)
	{
		UpdateManager.Subscribe(this);
		_countOfExecution = countOfExecution;
	}

	public void ManualUpdate()
	{
		var executions = 0;
		while (++executions != _countOfExecution && Tasks.TryDequeue(out var task))
		{
			task.Invoke();
		}
	}

	public override void Dispose()
	{
		UpdateManager.Unsubscribe(this);
	}
}