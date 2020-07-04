using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public struct PriorGenTaskJobHandle : IPriorGenTaskHandle
{
    private readonly JobHandle jobHandle;

    public PriorGenTaskJobHandle(JobHandle jobHandle)
    {
        this.jobHandle = jobHandle;
    }

    public void Complete()
    {
        jobHandle.Complete();
    }
}
