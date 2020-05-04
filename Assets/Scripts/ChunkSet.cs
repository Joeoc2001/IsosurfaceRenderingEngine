using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSet : MonoBehaviour
{
    private class ChunkPriority : IComparable<ChunkPriority>
    {
        private readonly double lastUpdatedTime;

        public ChunkPriority(double lastUpdatedTime)
        {
            this.lastUpdatedTime = lastUpdatedTime;
        }

        public int CompareTo(ChunkPriority other)
        {
            return lastUpdatedTime.CompareTo(other.lastUpdatedTime);
        }
    }

    public Chunk baseChunk;

    private int activeViewDistance;

    [Range(1, 100)]
    public int updatesPerTick = 10;

    [Range(1, 10)]
    public int viewDistance;

    [Range(0, 50)]
    public float normalApproxDistance;

    [Range(0, 2)]
    public float qualityDropoff;

    private Dictionary<Int3, Chunk> chunks;

    private Priority_Queue.SimplePriorityQueue<Chunk, ChunkPriority> updatePriority;

    // Start is called before the first frame update
    void Start()
    {
        chunks = new Dictionary<Int3, Chunk>();
        updatePriority = new Priority_Queue.SimplePriorityQueue<Chunk, ChunkPriority>();
    }

    bool ShouldApproximateNormals(double dist)
    {
        return dist >= normalApproxDistance;
    }

    int GetQuality(double dist)
    {
        return (int)Math.Max(1,
                baseChunk.quality - (qualityDropoff * dist) + 0.5
            );
    }

    public void RegenerateChunks(int dist)
    {
        if (dist == activeViewDistance)
        {
            return;
        }

        Dictionary<Int3, Chunk> newChunks = new Dictionary<Int3, Chunk>();

        Int3 baseIndex = new Int3(transform.position / baseChunk.size);
        for (int x = -dist; x < dist; x++)
        {
            for (int y = -dist; y < dist; y++)
            {
                for (int z = -dist; z < dist; z++)
                {
                    Int3 index = baseIndex + new Int3(x, y, z);

                    if (chunks.TryGetValue(index, out Chunk chunk))
                    {
                        chunks.Remove(index);
                    }
                    else
                    {
                        Vector3 offset = index * baseChunk.size;
                        chunk = Instantiate<Chunk>(baseChunk, offset, Quaternion.identity, transform);

                        updatePriority.Enqueue(chunk, new ChunkPriority(0));
                    }

                    newChunks.Add(index, chunk);
                }
            }
        }

        foreach (Int3 index in chunks.Keys)
        {
            Destroy(chunks[index].gameObject);
            updatePriority.Remove(chunks[index]);
        }

        chunks = newChunks;

        activeViewDistance = dist;
    }

    // Update is called once per frame
    void Update()
    {
        RegenerateChunks(viewDistance);

        for (int i = 0; i < updatesPerTick; i++)
        {
            Chunk chunk = updatePriority.Dequeue();

            float dist = chunk.transform.localPosition.magnitude;

            chunk.approximateNormals = baseChunk.approximateNormals || ShouldApproximateNormals(dist);
            chunk.quality = GetQuality(dist);

            chunk.GenerateMesh();

            updatePriority.Enqueue(chunk, new ChunkPriority(Time.time));
        }
    }
}