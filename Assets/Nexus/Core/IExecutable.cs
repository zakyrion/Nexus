using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IExecutable
{
	Task<object> Run(Message message);
}