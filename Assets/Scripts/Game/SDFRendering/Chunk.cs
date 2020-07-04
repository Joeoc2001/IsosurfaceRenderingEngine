using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chunk : MonoBehaviour
{
    public static readonly float SIZE = 10;

    public ChunkSet OwningSet;

    public abstract void SetQuality(int quality);

    public abstract PriorGenTask CreatePriorJob(SDF sdf);
}
