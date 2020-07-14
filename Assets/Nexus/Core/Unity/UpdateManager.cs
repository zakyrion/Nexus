using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
	private static UpdateManager _manager;

	private readonly List<IUpdateble> _updatebles = new List<IUpdateble>();
	private readonly HashSet<IUpdateble> _hash = new HashSet<IUpdateble>();

	private readonly ConcurrentQueue<IUpdateble> _addQueue = new ConcurrentQueue<IUpdateble>();
	private readonly ConcurrentQueue<IUpdateble> _removeQueue = new ConcurrentQueue<IUpdateble>();

	// Start is called before the first frame update
	private void Awake()
	{
		_manager = this;
		DontDestroyOnLoad(_manager);
	}

	// Update is called once per frame
	private void Update()
	{
		while (_addQueue.TryDequeue(out var entity))
		{
			_manager._hash.Add(entity);
			_manager._updatebles.Add(entity);
		}

		while (_removeQueue.TryDequeue(out var entity))
		{
			_manager._hash.Remove(entity);
			_manager._updatebles.Remove(entity);
		}

		foreach (var updateble in _updatebles) updateble.ManualUpdate();
	}

	#region API

	public static void Run()
	{
		if (_manager == null && Thread.CurrentThread.ManagedThreadId == 1)
		{
			var go = new GameObject("UpdateManager");
			go.hideFlags = HideFlags.HideInHierarchy;
			_manager = go.AddComponent<UpdateManager>();
		}
	}

	public static void Subscribe(IUpdateble entity)
	{
		_manager._addQueue.Enqueue(entity);
	}

	public static void Unsubscribe(IUpdateble entity)
	{
		_manager._removeQueue.Enqueue(entity);
	}

	#endregion
}