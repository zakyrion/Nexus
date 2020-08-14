using System;
using System.Threading.Tasks;
using Nexus.Core;
using UnityEngine;

public class Node : Actor<Node.Invoker>
{
	#region Node API

	protected ChannelTree ChannelTree;

	public override IInvoker InvokerCurrent => new Invoker(this);

	#endregion

	#region Invoker

	public sealed class Invoker : InvokerBase<Node>
	{
		public Invoker(Node node) : base(node)
		{
		}

		public void AddActor(Address address, IInvokable actor)
		{
			Actor.Invoke(Actor.AddActorHandler, address, actor);
		}

		public void RemoveActor(Address address)
		{
			Actor.Invoke(Actor.RemoveActorHandler, address);
		}

		public async Task<T> GetActor<T>(Address address) where T : IInvoker
		{
			var result = await Actor.Invoke(Actor.GetActorHandler<T>, address);
			return result;
		}
	}

	#endregion

	public override void OnAdd(Address address, Invoker parent)
	{
		base.OnAdd(address, parent);
		ChannelTree = new ChannelTree(address, this);
	}

	#region API
	
	protected void AddActorHandler(Address address, IInvokable invokable)
	{
		if (ChannelTree.Add(address, invokable))
			invokable.OnAdd(address, new Invoker(this));			
	}
	
	protected void RemoveActorHandler(Address address)
	{
		ChannelTree.Remove(address);
	}
	
	public T GetActorHandler<T>(Address address) where T : IInvoker
	{
		var actor = ChannelTree.Get(address);
		Debug.Log($"Actor: {actor} at path: {address} Type: {typeof(T)}");
		return actor != null ? actor.GetInvoker<T>() : default(T);
	}

	#endregion

	protected Node(IExecutor executor) : base(executor)
	{
	}
}