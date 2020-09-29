using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

namespace SDFRendering
{
    public abstract class GenTask
    {
        public abstract IPriorGenTaskHandle Schedule();
    }
}