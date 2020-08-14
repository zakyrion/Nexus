using System.Collections.Generic;
using System.Threading;
using Nexus.Core;
using UnityEngine;

public class ChannelTree
{
	private readonly Node _mainNode;

	private ChannelTree(Node mainNode)
	{
		_mainNode = mainNode;
	}

	public ChannelTree(Address endpoint, IInvokable actor)
	{
		_mainNode = new Node(endpoint, actor);
	}

	public bool Add(Address address, IInvokable actor)
	{
		return _mainNode.AddActor(address, actor);
	}

	public bool Remove(Address address)
	{
		return _mainNode.RemoveActor(address);
	}

	public IInvokable Get(Address address)
	{
		return _mainNode.Get(address);
	}

	public void Shootdown()
	{
		_mainNode.Shootdown();
	}

	private class Node
	{
		private IInvokable _invokable;
		public Address Address { get; }
		private Node _parentNode;
		private readonly Dictionary<string, Node> _childrenNodes = new Dictionary<string, Node>();

		public Node(Address address, Node parentNode)
		{
			Address = address;
			_parentNode = parentNode;
		}

		public Node(Address address, Node parentNode, IInvokable invokable)
		{
			Address = address;
			_parentNode = parentNode;
			_invokable = invokable;
		}

		public Node(Address address, IInvokable invokable)
		{
			Address = address;
			_invokable = invokable;
		}

		public Node GetNodeOrCreateByPath(Address address, int depth = 0)
		{
			if (depth > 100)
			{
				Debug.LogError("Recursion");
				return this;
			}

			if (Address != address)
			{
				string next = null;

				if (!string.IsNullOrEmpty(Address.Path))
					next = address.IsStartFrom(Address) ? address.GetNext(Address) : address.GetFirst();
				else
					next = address.GetFirst();

				if (!_childrenNodes.TryGetValue(next, out var nextNode))
				{
					var nextAddress = string.IsNullOrEmpty(Address.Path)
						? new Address(next)
						: new Address($"{Address.Path}/{next}");
					
					Debug.Log($"Add node to path: {nextAddress}");
					nextNode = new Node(nextAddress, this);
					_childrenNodes.Add(next, nextNode);
				}

				return nextNode.GetNodeOrCreateByPath(address, depth + 1);
			}

			return this;
		}

		public IInvokable Get(Address address)
		{
			if (Address != address)
			{
				string next = null;

				if (!string.IsNullOrEmpty(Address.Path))
					next = address.IsStartFrom(Address) ? address.GetNext(Address) : address.GetFirst();
				else
					next = address.GetFirst();

				//Debug.Log($"Next: {next} this: {Address}");
				
				if (!_childrenNodes.TryGetValue(next, out var nextNode))
				{
					/*foreach (var node in _childrenNodes)
					{
						Debug.Log($"Key: {node.Key}");
					}*/
					return null;
				}

				return nextNode.Get(address);
			}
			
			//Debug.Log("Return");
			return _invokable;
		}

		public bool AddActor(Address address, IInvokable actor)
		{
			var node = GetNodeOrCreateByPath(address);
			Debug.Log($"add actor to: {node.Address}");
			node._invokable = actor;
			return true;
		}

		public bool RemoveActor(Address address)
		{
			if (Address != address)
			{
				var next = address.GetNext(Address);

				if (!_childrenNodes.TryGetValue(next, out var nextNode))
					return false;

				nextNode.RemoveActor(address);
			}

			_invokable.Dispose();
			_invokable = null;
			return true;
		}

		public bool AddNode(Address address, Node node)
		{
			var parent = GetNodeOrCreateByPath(address);

			foreach (var childrenNode in node._childrenNodes)
			{
				if (!parent._childrenNodes.ContainsKey(childrenNode.Key))
					parent._childrenNodes[childrenNode.Key] = childrenNode.Value;
			}

			parent._invokable = node._invokable;
			return true;
		}

		public void Shootdown()
		{
			_invokable?.Dispose();

			foreach (var node in _childrenNodes)
			{
				node.Value.Shootdown();
			}
		}

		public static implicit operator ChannelTree(Node node)
		{
			return new ChannelTree(node);
		}

		public static implicit operator Node(ChannelTree tree)
		{
			return tree._mainNode;
		}
	}
}