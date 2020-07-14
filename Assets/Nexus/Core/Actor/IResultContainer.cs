using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResultContainer
{
	void SetResult(object result);
	void Fail();
}