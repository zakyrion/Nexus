using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoActor<T> : MonoBehaviour where T: IInvoker, new()
{
    protected Actor<T> _actor;

    public MonoActor()
    {
    }
    
    #region BindManager
    
    #endregion
}
