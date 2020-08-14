using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switcher<T>
{
	public T Val
	{
		get => _data;
		set
		{
			lock (_lock)
			{
				_data = value;
			}
		}
	}

	private T _data;
	object _lock = new object();
}