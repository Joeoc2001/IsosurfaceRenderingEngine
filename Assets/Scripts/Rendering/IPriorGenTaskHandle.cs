using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace SDFRendering
{
    public interface IPriorGenTaskHandle
    {
        void Complete();
    }
}