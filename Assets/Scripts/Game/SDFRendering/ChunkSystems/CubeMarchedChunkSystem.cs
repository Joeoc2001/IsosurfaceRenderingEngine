using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMarchedChunkSystem : ChunkManagementSystem
{
    public CubeMarchedChunk templateChunk;

    [Range(0, 10)]
    public int baseQuality;

    [Range(0, 2)]
    public float qualityDropoff;

    public override int GetChunkQuality(ChunkSet owner, Chunk chunk)
    {
        Vector3Int? offsetQuery = owner.GetChunkOffset(chunk);
        if (!offsetQuery.HasValue)
        {
            return 0; // If the chunk isn't present then it can have any value of quality we want
        }

        Vector3Int offset = offsetQuery.Value;
        int dist = Math.Max(Math.Max(offset.x, offset.y), offset.z);
        return (int)Math.Max(1, baseQuality - (qualityDropoff * dist) + 0.5);
    }

    public override Chunk InstantiateNewChunk(ChunkSet destination, Vector3Int index)
    {
        CubeMarchedChunk newChunk = Instantiate(templateChunk, destination.transform, false);
        return newChunk;
    }

    public override bool ShouldChunkBeReinstantiated(Chunk chunk)
    {
        return false; // All chunks are the same so the chunk type should never be changed
    }
}
