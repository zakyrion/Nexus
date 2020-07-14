using System;
using Nexus.Core;

public interface IInvokable : IDisposable
{
	T GetInvoker<T>() where T : IInvoker;
	void OnAdd(Address address, Node.Invoker parent);
}