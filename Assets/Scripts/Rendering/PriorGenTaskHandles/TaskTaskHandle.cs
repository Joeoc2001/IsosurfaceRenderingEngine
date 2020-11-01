using SDFRendering;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TaskTaskHandle : ITaskHandle
{
    private readonly Task _task;

    public TaskTaskHandle(Task task)
    {
        _task = task;
    }

    public void Complete()
    {
        _task.Wait();
        _task.Dispose();
    }

    public bool HasFinished()
    {
        return _task.IsCompleted || _task.IsFaulted || _task.IsCanceled;
    }
}
