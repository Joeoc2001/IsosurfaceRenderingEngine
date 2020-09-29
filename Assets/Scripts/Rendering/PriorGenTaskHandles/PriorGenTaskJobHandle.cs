using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace SDFRendering.JobHandles
{
    public struct PriorGenTaskJobHandle : IPriorGenTaskHandle
    {
        private readonly JobHandle _jobHandle;

        public PriorGenTaskJobHandle(JobHandle jobHandle)
        {
            this._jobHandle = jobHandle;
        }

        public void Complete()
        {
            _jobHandle.Complete();
        }
    }
}