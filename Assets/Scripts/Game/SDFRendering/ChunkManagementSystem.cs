using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkManagementSystem : MonoBehaviour
{
    public abstract bool ShouldChunkBeReinstantiated(Chunk chunk);

    public abstract Chunk InstantiateNewChunk(ChunkSet destination, Vector3Int index);

    public abstract int GetChunkQuality(ChunkSet owner, Chunk chunk);
}
