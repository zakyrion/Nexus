using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BehaviorTest : MonoBehaviour
{
	public static int Count = 1000000;
	// Start is called before the first frame update
	private async void Awake()
	{
		Core.RunCoreAtMainTread();
		//Core.RunCore();

		try
		{
			//var executor = new ThreadPoolExecutor();
			var executor = new ThreadExecutor();
			
			Core.AddActor("main/one", new ActorOne(executor));
			Core.AddActor("main/two", new ActorTwo(executor));
			
			var actor = await Core.GetActor<ActorOne.Invoker>("main/one");
			Debug.Log($"Actor: {actor}");
			for (int i = 0; i != Count + 1; i++)
			{
				actor.SumInvoke(i,10);	
			}
			
			Debug.LogWarning("Add all");
			
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
	}

	void OnDisable()
	{
		Core.StopCore();
	}

	public void OnApplicationQuit()
	{
		Core.StopCore();		
	}
}

public class ActorOne : Actor<ActorOne.Invoker>
{
	public override IInvoker InvokerCurrent => new Invoker(this);

	private Stopwatch _stopwatch;
	public class Invoker : InvokerBase<ActorOne>
	{
		public Invoker(ActorOne actor):base(actor)
		{
		}

		public void SumInvoke(int x, int y)
		{
			Actor.Invoke(Actor.Sum, x, y);
		}

		public void Send(Node.Invoker parent)
		{
			parent.GetActor<ActorTwo.Invoker>("main/two").ContinueWith(task =>
			{
				Debug.Log($"Actor found: {task.Result}");
				task.Result.DisplayMessage();
			});
		}

		public async void SendTwo(Node.Invoker parent)
		{
			var actor = await parent.GetActor<ActorTwo.Invoker>("main/two");
			actor.DisplayMessage();
		}
	}

	public ActorOne(IExecutor executor) : base(executor)
	{
	}

	void Sum(int x, int y)
	{
		if (x == 0)
		{
			Debug.Log("Start sum");
			_stopwatch = Stopwatch.StartNew();
		}

		//Debug.Log($"Sum Thread id: {Thread.CurrentThread.ManagedThreadId}");
		
		if (x == BehaviorTest.Count)
		{
			_stopwatch.Stop();
			Debug.LogWarning($"Spend ms: {_stopwatch.ElapsedMilliseconds} Spend ticks: {_stopwatch.ElapsedTicks} at thread: {Thread.CurrentThread.ManagedThreadId}");
			
			GetInvoker().Send(Parent);
		}
		//Debug.Log($"Thread: {Thread.CurrentThread.ManagedThreadId}");
		//Debug.Log($"sum: {x + y}");
	}
}

public class ActorTwo : Actor<ActorTwo.Invoker>
{
	public override IInvoker InvokerCurrent => new Invoker(this);

	public class Invoker : InvokerBase<ActorTwo>
	{
		public Invoker(ActorTwo actor):base(actor)
		{
		}

		public void DisplayMessage()
		{
			Actor.Invoke(Actor.DisplayMessageHandler);
		}

		public async void Restart(Node.Invoker parent)
		{
			var actor = await parent.GetActor<ActorOne.Invoker>("main/one");
			await Task.Delay(1000);
			
			Debug.Log($"Restart: {actor}");
			
			for (int i = 0; i != BehaviorTest.Count + 1; i++)
				actor.SumInvoke(i, 15);
		}
	}

	public ActorTwo(IExecutor executor) : base(executor)
	{
	}
	void DisplayMessageHandler()
	{
		Debug.Log("Display");
		GetInvoker().Restart(Parent);
	}
}