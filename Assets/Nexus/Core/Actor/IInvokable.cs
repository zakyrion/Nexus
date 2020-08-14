using System;
using Nexus.Core;

public interface IInvokable : IAdressable, IDisposable
{
	T GetInvoker<T>() where T : IInvoker;
	void OnAdd(Address address, Node.Invoker parent);
}