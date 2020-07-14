using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nexus.Core;
using UnityEngine;
using UnityEngine.PlayerLoop;

public sealed class Core : Node
{
	private static Core _core;

	#region Static API

	public static void RunCore()
	{
		if (_core == null)
		{
			UpdateManager.Run();
			_core = new Core(new ThreadExecutor());
		}
	}

	public static void RunCoreAtMainTread()
	{
		if (_core == null)
		{
			UpdateManager.Run();
			_core = new Core(new UnityExecutor());
		}
	}

	public static void StopCore()
	{
		_core?.StopCoreInternal();
	}

	public static void AddActor(Address address, IInvokable invokable)
	{
		_core.Invoke(_core.AddActorInternal, address, invokable);
	}

	public static void RemoveActor(Address address)
	{
		_core.Invoke(_core.RemoveActorInternal, address);
	}

	public static async Task<T> GetActor<T>(Address address) where T : IInvoker
	{
		var result = await _core.Invoke(_core.GetActorInternal<T>, address);
		return result;
	}

	#endregion

	private void AddActorInternal(Address address, IInvokable invokable)
	{
		if (_core == null)
			RunCoreAtMainTread();

		_core.AddActorHandler(address, invokable);
	}

	private void RemoveActorInternal(Address address)
	{
		if (_core == null)
			RunCoreAtMainTread();

		_core.RemoveActorHandler(address);
	}

	private T GetActorInternal<T>(Address address) where T : IInvoker
	{
		if (_core == null)
			RunCoreAtMainTread();

		return GetActorHandler<T>(address);
	}

	private void StopCoreInternal()
	{
		ChannelTree.Shootdown();
	}

	private Core(IExecutor executor) : base(executor)
	{
		OnAdd(new Address(null), null);
	}
}