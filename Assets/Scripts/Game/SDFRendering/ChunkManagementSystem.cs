using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkManagementSystem : MonoBehaviour
{
    public abstract bool ShouldChunkBeReinstantiated(Chunk chunk);

    public abstract bool ShouldChunkBeKilled(Chunk chunk);

    public abstract Chunk InstantiateNewChunk(ChunkSet destination, Vector3Int index, Vector3 position);

    public abstract int GetChunkQuality(Chunk chunk);
}
