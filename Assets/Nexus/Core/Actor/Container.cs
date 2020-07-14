using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container<T>
{
	public T Param1 { get; set; }
}

public class Container<T,R>
{
	public T Param1 { get; set; }
	public R Param2 { get; set; }
}

public class Container<T, R, K>
{
	public T Param1 { get; set; }
	public R Param2 { get; set; }
	public K Param3 { get; set; }
}