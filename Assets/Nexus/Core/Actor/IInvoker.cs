public interface IInvoker
{
	T Convert<T>() where T : IInvoker;
	void Quit();
}

public abstract class InvokerBase<T> : IInvoker where T: IInvokable
{
	protected T Actor;
	protected InvokerBase(T actor)
	{
		Actor = actor;
	}

	public virtual T Convert<T>() where T : IInvoker
	{
		return (T) (IInvoker) this;
	}

	public virtual void Quit()
	{
		Actor?.Dispose();
	}
}