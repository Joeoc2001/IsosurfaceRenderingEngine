using Algebra;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Assertions;

public class ChunkSet : MonoBehaviour
{
    public static readonly double DELTA_RATIO = 1;

    private class ChunkContainer : Priority_Queue.FastPriorityQueueNode
    {
        private float lastUpdatedTime;
        public readonly Chunk Chunk;

        public ChunkContainer(Chunk chunk)
        {
            lastUpdatedTime = 0;
            Chunk = chunk;
            SetPriority();
        }

        public void SetLastUpdatedTime()
        {
            lastUpdatedTime = Time.time;
            SetPriority();
        }

        private void SetPriority()
        {
            float priority = lastUpdatedTime;
            Priority = priority;
        }
    }

    public Chunk baseChunk;

    public EquationProvider distanceField;

    public int lastUpdates = 0;
    public double updatesPerSecond = 0;

    [Range(0, 2)]
    public double updatesPerChunkSecond = 0.1;

    [Range(0, 10000)]
    public int maxUpdatesPerSecond = 5000;

    [Range(1, 50)]
    public int viewDistance;

    [Range(0, 50)]
    public float normalApproxDistance;

    [Range(0, 2)]
    public float qualityDropoff;

    public Vector3Int baseIndex;

    private TwoWayDict<Vector3Int, Chunk> chunks;

    private Priority_Queue.FastPriorityQueue<ChunkContainer> updatePriority;

    // Start is called before the first frame update
    void Start()
    {
        chunks = new TwoWayDict<Vector3Int, Chunk>();
        updatePriority = new Priority_Queue.FastPriorityQueue<ChunkContainer>(GetMaxChunks());
    }

    private int GetMaxChunks()
    {
        int side = (2 * viewDistance) + 1;
        return side * side * side;
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

    private static IEnumerable<Vector3Int> GetNewChunkIndexes(Vector3Int oldCenter, int oldR, Vector3Int newCenter, int newR)
    {
        List<Vector3Int> newChunks = new List<Vector3Int>();

        if (oldCenter == newCenter && oldR >= newR)
        {
            return newChunks;
        }

        for (int x = newCenter.x - newR; x <= newCenter.x + newR; x++)
        {
            for (int y = newCenter.y - newR; y <= newCenter.y + newR; y++)
            {
                // The abstraction for this bit is to imagine the projection of the two cubes
                // onto the x-y plane and check if we are in the intersection first
                int minZ = newCenter.y - newR;
                int maxZ = newCenter.y + newR;

                if (x < oldCenter.x + oldR && x > oldCenter.x - oldR
                    && y < oldCenter.y + oldR && y > oldCenter.y - oldR) // In bounds
                {
                    minZ = Math.Max(minZ, oldCenter.z - oldR);
                    maxZ = Math.Min(maxZ, oldCenter.z + oldR);
                }

                for (int z = minZ; z <= maxZ; z++)
                {
                    newChunks.Add(new Vector3Int(x, y, z));
                }
            }
        }

        return newChunks;
    }

    private Vector3Int generatedBaseIndex = new Vector3Int(0, 0, 0);
    private int generatedViewDistance = 0;
    public void GenerateChunks()
    {
        IEnumerable<Vector3Int> newChunkIndexes = GetNewChunkIndexes(generatedBaseIndex, generatedViewDistance, baseIndex, viewDistance);

        // Check if priority queue needs resizing
        if (viewDistance > generatedViewDistance || viewDistance < generatedViewDistance / 1.2)
        {
            updatePriority.Resize(GetMaxChunks());
        }

        foreach (Vector3Int index in newChunkIndexes)
        {
            if (!chunks.ContainsKey(index))
            {
                Vector3 offset = new Vector3(index.x, index.y, index.z) * baseChunk.size;
                Chunk chunk = Instantiate<Chunk>(baseChunk, offset, Quaternion.identity, transform);

                ChunkContainer container = new ChunkContainer(chunk);
                updatePriority.Enqueue(container, container.Priority);

                chunks.Add(index, chunk);
            }
            //Assert.IsNotNull(chunks[index]);
        }

        generatedBaseIndex = baseIndex;
        generatedViewDistance = viewDistance;
    }

    private readonly Dictionary<ChunkContainer, (JobHandle handle, FeelerNodeSetJob job)> chunkFeelerNodeJobs
        = new Dictionary<ChunkContainer, (JobHandle handle, FeelerNodeSetJob job)>();

    private List<(ChunkContainer, FeelerNodeSet)> FinishFeelerNodeJobs()
    {
        List<(ChunkContainer, FeelerNodeSet)> data
            = new List<(ChunkContainer, FeelerNodeSet)>(chunkFeelerNodeJobs.Count);

        foreach (ChunkContainer chunkContainer in chunkFeelerNodeJobs.Keys)
        {
            (JobHandle handle, FeelerNodeSetJob job) = chunkFeelerNodeJobs[chunkContainer];
            handle.Complete();

            // Extract completed datas
            FeelerNode[] feelerNodes = new FeelerNode[job.Target.Length];
            job.Target.CopyTo(feelerNodes);
            int resolution = job.Resolution;
            FeelerNodeSet feelerNodeSet = new FeelerNodeSet(resolution, feelerNodes);

            // Clean up
            job.Target.Dispose();

            // Add chunk to be modified
            data.Add((chunkContainer, feelerNodeSet));

        }

        chunkFeelerNodeJobs.Clear();

        return data;
    }

    private bool ShouldChunkBeKilled(Chunk chunk)
    {
        Vector3Int index = chunks[chunk] - baseIndex;
        int dist = Math.Max(Math.Max(index.x, index.y), index.z);
        return dist > viewDistance + 1;
    }

    private FeelerNodeSetJob CreateFeelerNodeJob(ChunkContainer chunkContainer, Equation.ExpressionDelegate distFunc)
    {
        Chunk chunk = chunkContainer.Chunk;
        int resolution = (1 << chunk.quality) + 1;
        return new FeelerNodeSetJob()
        {
            // Get a pointer to the distance function
            Function = new FunctionPointer<Equation.ExpressionDelegate>(Marshal.GetFunctionPointerForDelegate(distFunc)),

            Resolution = resolution,
            Delta = chunk.size / (1 << chunk.quality),
            Origin = chunk.transform.position,

            Target = new NativeArray<FeelerNode>(resolution * resolution * resolution, Allocator.TempJob)
        };
    }

    private void RandomTickChunk(ChunkContainer chunkContainer, Equation.ExpressionDelegate distFunc)
    {
        Chunk chunk = chunkContainer.Chunk;
        Vector3Int index = chunks[chunk];
        float dist = (index - baseIndex).magnitude;

        // Check to see if the chunk should be deleted
        if (ShouldChunkBeKilled(chunk))
        {
            chunks.Remove(index);
            Destroy(chunk.gameObject);
            return;
        }

        // Update fields
        chunk.approximateNormals = baseChunk.approximateNormals || ShouldApproximateNormals(dist);
        chunk.quality = GetQuality(dist);

        // Create new feeler node update job
        FeelerNodeSetJob feelerNodeSetJob = CreateFeelerNodeJob(chunkContainer, distFunc);
        JobHandle handle = feelerNodeSetJob.Schedule();
        chunkFeelerNodeJobs.Add(chunkContainer, (handle, feelerNodeSetJob));
    }

    private int GetNumberOfChunksToTick()
    {
        int updates = (int)(updatesPerChunkSecond * Time.deltaTime * updatePriority.Count);
        updates = Math.Min(updates, updatePriority.Count);
        updates = Math.Min(updates, (int)(maxUpdatesPerSecond * Time.deltaTime));
        return updates;
    }

    private void UpdateMetrics(int updates)
    {
        lastUpdates = updates;

        double ratioScaled = DELTA_RATIO * Time.deltaTime;
        updatesPerSecond *= (1 - ratioScaled);
        updatesPerSecond += (updates / Time.deltaTime) * ratioScaled;
    }

    public void TickChunks()
    {
        // Calculate functions
        Equation distEq = distanceField.GetEquation();
        Equation.ExpressionDelegate distFunc = distEq.GetExpression();
        Equation.Vector3ExpressionDelegate normFunc = distEq.GetDerivitiveExpressionWrtXYZ();

        // Complete all feeler node tasks
        List<(ChunkContainer, FeelerNodeSet)> chunkNodes = FinishFeelerNodeJobs();
        foreach ((ChunkContainer chunkContainer, FeelerNodeSet nodes) in chunkNodes)
        {
            // Update chunk's mesh
            chunkContainer.Chunk.GenerateMesh(nodes, normFunc);
            chunkContainer.SetLastUpdatedTime();
            updatePriority.Enqueue(chunkContainer, chunkContainer.Priority);
        }

        int updates = GetNumberOfChunksToTick();
        // Loop through some and update
        for (int i = 0; i < updates; i++)
        {
            ChunkContainer chunkContainer = updatePriority.Dequeue();
            RandomTickChunk(chunkContainer, distFunc);
        }

        UpdateMetrics(updates);
    }

    // Update is called once per frame
    void Update()
    {
        GenerateChunks(); // Checks which chunks should exist given the position etc of this set
        TickChunks(); // Updates a collection of the chunks
    }

    void OnDestroy()
    {
        // Clean up any leftover tasks
        FinishFeelerNodeJobs();
    }
}