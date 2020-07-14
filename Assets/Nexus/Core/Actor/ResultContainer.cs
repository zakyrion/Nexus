using System.Threading.Tasks;

public class ResultContainer<T> : IResultContainer
{
	private readonly TaskCompletionSource<T> _task;

	public ResultContainer(TaskCompletionSource<T> task)
	{
		_task = task;
	}

	public void SetResult(object result)
	{
		_task.SetResult((T) result);
	}

	public void Fail()
	{
		_task.SetCanceled();
	}
}