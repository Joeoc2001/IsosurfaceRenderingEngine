using SDFRendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneTaskHandle : IPriorGenTaskHandle
{
    public void Complete()
    {
        return;
    }
}
