using System.Threading;
using System.Threading.Tasks;
using Nexus.Core;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"Start: {SynchronizationContext.Current}");

        var bind = new BindManager(new ThreadPoolExecutor());

        for (int i = 0; i < 3; i++)
        {
            var msgOne = MessagePool.GetMessage(); 
            msgOne.Init(new Address(), new Address(), 15);
        }
        
        for (int i = 0; i < 10; i++)
        {
            TaskCompletionSource<float> result = new TaskCompletionSource<float>();
            var rqOne = MessagePool.GetMessage(); 
            rqOne.Init(new Address(), new Address(), (float)i, Message.DeliveryType.ToOne,new ResultContainer<float>(result));
        }
        
        Debug.Log($"Finish: {SynchronizationContext.Current}");
    }
    
    public void CallTwo(int someParam)
    {
        Debug.Log("Call two");
    }

    public async Task<bool> CallOne(int someParam)
    {
        Debug.Log($"Call one: {Thread.CurrentThread.ManagedThreadId} Context: {SynchronizationContext.Current}");
        var result = await Await();
        Debug.Log($"After Await: {Thread.CurrentThread.ManagedThreadId} Context: {SynchronizationContext.Current}");
        return result;
    }

    public async Task<bool> Await()
    {
        Debug.Log($"Await: {Thread.CurrentThread.ManagedThreadId} Context: {SynchronizationContext.Current}");
        //await Task.Delay(5000);
        for (int i = 0; i < 10; i++)
        {
            if (Monitor.IsEntered(_lock))
            {
                Debug.LogError("Entered");
            }
            
            _someVar++;
            
            await Task.Delay(100);
        }
        Debug.Log($"After delay: {Thread.CurrentThread.ManagedThreadId} Context: {SynchronizationContext.Current}");
        return true;
    }

    public int RequestOne(float f)
    {
        if (Monitor.IsEntered(_lock))
        {
            Debug.LogWarning("Entered");
        }
        else
        {
            Monitor.Enter(_lock);
            
            Debug.LogWarning(
                $"Ask: {_someVar} Id: {Thread.CurrentThread.ManagedThreadId} Context: {SynchronizationContext.Current}");
            
            Monitor.Exit(_lock);
        }

        return 1;
    }

    private float _someVar;
    object _lock = new Object();
}
