using System;
using System.Threading.Tasks;

#region Actions
public class ActionContainer : IActionContainer
{
	private Action _action;

	public ActionContainer(Action action)
	{
		_action = action;
	}
		
	public void Invoke()
	{
		_action?.Invoke();
	}
}

public class ActionContainer<T> : IActionContainer
{
	private Action<T> _action;
	private T _param;

	public ActionContainer(Action<T> action, T param)
	{
		_action = action;
		_param = param;
	}
		
	public void Invoke()
	{
		_action?.Invoke(_param);
	}
}

public class ActionContainer<T, T1> : IActionContainer
{
	private Action<T,T1> _action;
	private T _param;
	private T1 _param1;

	public ActionContainer(Action<T,T1> action, T param, T1 param1)
	{
		_action = action;
		_param = param;
		_param1 = param1;
	}
		
	public void Invoke()
	{
		_action?.Invoke(_param, _param1);
	}
}

public class ActionContainer<T, T1, T2> : IActionContainer
{
	private Action<T,T1,T2> _action;
	private T _param;
	private T1 _param1;
	private T2 _param2;

	public ActionContainer(Action<T,T1,T2> action, T param, T1 param1, T2 param2)
	{
		_action = action;
		_param = param;
		_param1 = param1;
		_param2 = param2;
	}
		
	public void Invoke()
	{
		_action?.Invoke(_param, _param1, _param2);
	}
}

#endregion

#region Functions

public class FunctionContainer<R> : IActionContainer
{
	private Func<R> _function;
	private TaskCompletionSource<R> _completionSource;

	public FunctionContainer(Func<R> function, TaskCompletionSource<R> completionSource)
	{
		_function = function;
		_completionSource = completionSource;
	}

	public void Invoke()
	{
		_completionSource.SetResult(_function.Invoke());
	}
}
public class FunctionContainer<R, T> : IActionContainer
{
	private Func<T,R> _function;
	private TaskCompletionSource<R> _completionSource;
	private T _param;

	public FunctionContainer(Func<T,R> function, TaskCompletionSource<R> completionSource, T param)
	{
		_function = function;
		_completionSource = completionSource;
		_param = param;
	}

	public void Invoke()
	{
		_completionSource.SetResult(_function.Invoke(_param));
	}
}

public class FunctionContainer<R, T, T1> : IActionContainer
{
	private Func<T,T1,R> _function;
	private TaskCompletionSource<R> _completionSource;
	private T _param;
	private T1 _param1;

	public FunctionContainer(Func<T,T1,R> function, TaskCompletionSource<R> completionSource, T param, T1 param1)
	{
		_function = function;
		_completionSource = completionSource;
		_param = param;
		_param1 = param1;
	}

	public void Invoke()
	{
		_completionSource.SetResult(_function.Invoke(_param, _param1));
	}
}

public class FunctionContainer<R, T, T1, T2> : IActionContainer
{
	private Func<T,T1,T2, R> _function;
	private TaskCompletionSource<R> _completionSource;
	private T _param;
	private T1 _param1;
	private T2 _param2;

	public FunctionContainer(Func<T,T1,T2,R> function, TaskCompletionSource<R> completionSource, T param, T1 param1, T2 param2)
	{
		_function = function;
		_completionSource = completionSource;
		_param = param;
		_param1 = param1;
		_param2 = param2;
	}

	public void Invoke()
	{
		_completionSource.SetResult(_function.Invoke(_param, _param1, _param2));
	}
}

#endregion