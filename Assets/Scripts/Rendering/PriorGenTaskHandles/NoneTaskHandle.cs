using SDFRendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneTaskHandle : ITaskHandle
{
    public void Complete()
    {
        return;
    }

    public bool HasFinished()
    {
        return true;
    }
}
