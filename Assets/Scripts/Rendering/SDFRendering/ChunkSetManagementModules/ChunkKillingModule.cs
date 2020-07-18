using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkKillingModule : ChunkSetManagementModule
{
    private readonly FastRemovableQueue<Vector3Int?> checkQueue = new FastRemovableQueue<Vector3Int?>();

    public override void Init(ChunkSet set, ChunkSystem system)
    {
        set.OnChunkAdded += Set_OnChunkAdded;
        set.OnChunkRemoved += Set_OnChunkRemoved;
    }

    private void Set_OnChunkAdded(ChunkSet set, Chunk chunk, Vector3Int index)
    {
        checkQueue.Push(index);
    }

    private void Set_OnChunkRemoved(ChunkSet set, Chunk chunk, Vector3Int index)
    {
        checkQueue.Remove(index);
    }

    public override void Tick(ChunkSet set, ChunkSystem system)
    {
        int toTick = (int)(checkQueue.Length * Time.deltaTime);

        for (int i = 0; i < toTick; i++)
        {
            Vector3Int index = checkQueue.Pop().Value;
            if (system.ShouldChunkBeKilled(index))
            {
                Chunk chunk = set.GetChunkByIndex(index);
                set.RemoveChunk(index);
                Object.Destroy(chunk.gameObject);
            }
        }
    }

    public override void Destruct(ChunkSet set, ChunkSystem system)
    {
        set.OnChunkAdded -= Set_OnChunkAdded;
        set.OnChunkRemoved -= Set_OnChunkRemoved;
    }
}
