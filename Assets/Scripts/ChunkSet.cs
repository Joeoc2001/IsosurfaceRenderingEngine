using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

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

    [Range(1, 100)]
    public int updatesPerTick = 10;

    [Range(1, 10)]
    public int viewDistance;

    [Range(0, 50)]
    public float normalApproxDistance;

    [Range(0, 2)]
    public float qualityDropoff;

    public int3 baseIndex;

    private TwoWayDict<int3, Chunk> chunks;

    private Priority_Queue.SimplePriorityQueue<Chunk, ChunkPriority> updatePriority;

    // Start is called before the first frame update
    void Start()
    {
        chunks = new TwoWayDict<int3, Chunk>();
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

    public void GenerateChunks()
    {

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                for (int z = -viewDistance; z <= viewDistance; z++)
                {
                    int3 index = baseIndex + new int3(x, y, z);

                    if (!chunks.ContainsKey(index))
                    {
                        Vector3 offset = new Vector3(index.x, index.y, index.z) * baseChunk.size;
                        Chunk chunk = Instantiate<Chunk>(baseChunk, offset, Quaternion.identity, transform);

                        updatePriority.Enqueue(chunk, new ChunkPriority(0));

                        chunks.Add(index, chunk);
                    }
                    Assert.IsNotNull(chunks[index]);
                }
            }
        }
    }

    public void TickChunks()
    {
        for (int i = 0; i < updatesPerTick; i++)
        {
            Chunk chunk = updatePriority.Dequeue();

            int3 index = chunks[chunk];

            int3 absDist = math.abs(index - baseIndex);
            if (math.max(math.max(absDist.x, absDist.y), absDist.z) > viewDistance)
            {
                chunks.Remove(index);

                Destroy(chunk.gameObject);
                continue;
            }

            float dist = chunk.transform.localPosition.magnitude;

            chunk.approximateNormals = baseChunk.approximateNormals || ShouldApproximateNormals(dist);
            chunk.quality = GetQuality(dist);

            chunk.GenerateMesh();

            updatePriority.Enqueue(chunk, new ChunkPriority(Time.time));
        }
    }

    // Update is called once per frame
    void Update()
    {
        GenerateChunks();
        TickChunks();
    }
}