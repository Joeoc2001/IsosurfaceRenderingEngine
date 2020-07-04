using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

public abstract class PriorGenTask
{
    public abstract IPriorGenTaskHandle Schedule();

    public abstract void AfterFinished();
}
