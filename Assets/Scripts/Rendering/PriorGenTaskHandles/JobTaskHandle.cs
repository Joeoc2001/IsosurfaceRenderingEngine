using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace SDFRendering.JobHandles
{
    public struct JobTaskHandle : ITaskHandle
    {
        private readonly JobHandle _jobHandle;

        public JobTaskHandle(JobHandle jobHandle)
        {
            this._jobHandle = jobHandle;
        }

        public void Complete()
        {
            _jobHandle.Complete();
        }

        public bool HasFinished()
        {
            return _jobHandle.IsCompleted;
        }
    }
}