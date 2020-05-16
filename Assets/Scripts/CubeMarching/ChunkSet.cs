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

    public const int MAX_RADIUS = 50;

    public Chunk baseChunk;

    public EquationProvider distanceField;

    public int lastUpdates = 0;
    public double updatesPerSecond = 0;

    [Range(0, 2)]
    public double updatesPerChunkSecond = 0.1;

    [Range(0, 10000)]
    public int maxUpdatesPerSecond = 5000;

    [Range(1, MAX_RADIUS)]
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
        int maxChunks = MAX_RADIUS * MAX_RADIUS * MAX_RADIUS * 8;
        updatePriority = new Priority_Queue.FastPriorityQueue<ChunkContainer>(maxChunks);
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

        // Check if new chunks are entirely contained in old chunks
        if ((newCenter - oldCenter).magnitude + newR <= oldR)
        {
            return newChunks;
        }

        for (int x = -newR; x <= newR; x++)
        {
            float xPartR = Mathf.Sqrt(newR * newR - x * x);

            for (int y = -(int)xPartR; y <= (int)xPartR; y++)
            {
                int yPartR = (int)Mathf.Sqrt(xPartR * xPartR - y * y);
                
                for(int z = -yPartR; z <= yPartR; z++)
                {
                    Vector3Int index = newCenter + new Vector3Int(x, y, z);

                    if ((oldCenter - index).magnitude < oldR)
                    {
                        continue;
                    }

                    newChunks.Add(index);
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

    public void TickChunks()
    {
        // Calculate functions
        Equation distEq = distanceField.GetEquation();
        Equation.ExpressionDelegate distFunc = distEq.GetExpression();
        Equation.ExpressionDelegate dxFunc = distEq.GetDerivative(Variable.X).GetExpression();
        Equation.ExpressionDelegate dyFunc = distEq.GetDerivative(Variable.Y).GetExpression();
        Equation.ExpressionDelegate dzFunc = distEq.GetDerivative(Variable.Z).GetExpression();
        Func<VariableSet, Vector3> normFunc = v => new Vector3(dxFunc(v), dyFunc(v), dzFunc(v));

        // Find complete all feeler node tasks
        foreach (ChunkContainer chunkContainer in chunkFeelerNodeJobs.Keys)
        {
            (JobHandle handle, FeelerNodeSetJob job) = chunkFeelerNodeJobs[chunkContainer];
            handle.Complete();

            // Extract completed datas
            FeelerNode[] feelerNodes = job.Target.ToArray();
            int resolution = job.Resolution;
            FeelerNodeSet feelerNodeSet = new FeelerNodeSet(resolution, feelerNodes);

            // Update chunk
            chunkContainer.Chunk.GenerateMesh(feelerNodeSet, normFunc);

            // Clean up
            job.Target.Dispose();

            chunkContainer.SetLastUpdatedTime();
            updatePriority.Enqueue(chunkContainer, chunkContainer.Priority);
        }

        chunkFeelerNodeJobs.Clear();

        // Loop through some and update
        int updates = (int)(updatesPerChunkSecond * Time.deltaTime * updatePriority.Count);
        updates = Math.Min(updates, updatePriority.Count);
        updates = Math.Min(updates, (int)(maxUpdatesPerSecond * Time.deltaTime));
        for (int i = 0; i < updates; i++)
        {
            ChunkContainer chunkContainer = updatePriority.Dequeue();
            Chunk chunk = chunkContainer.Chunk;

            // Check to see if the chunk should be deleted
            Vector3Int index = chunks[chunk];
            float dist = (index - baseIndex).magnitude;
            if (dist > viewDistance + 1)
            {
                chunks.Remove(index);

                Destroy(chunk.gameObject);
                continue;
            }

            // Update fields
            chunk.approximateNormals = baseChunk.approximateNormals || ShouldApproximateNormals(dist);
            chunk.quality = GetQuality(dist);

            // Create new feeler node update job
            int resolution = (1 << chunk.quality) + 1;
            FeelerNodeSetJob feelerNodeSetJob = new FeelerNodeSetJob()
            {
                // Get a pointer to the distance function
                Function = new FunctionPointer<Equation.ExpressionDelegate>(Marshal.GetFunctionPointerForDelegate(distFunc)),

                Resolution = resolution,
                Delta = chunk.size / (1 << chunk.quality),
                Origin = chunk.transform.position,

                Target = new NativeArray<FeelerNode>(resolution * resolution * resolution, Allocator.TempJob)
            };
            JobHandle handle = feelerNodeSetJob.Schedule();
            chunkFeelerNodeJobs.Add(chunkContainer, (handle, feelerNodeSetJob));
        }
        lastUpdates = updates;

        double ratioScaled = DELTA_RATIO * Time.deltaTime;
        updatesPerSecond *= (1 - ratioScaled);
        updatesPerSecond += (updates / Time.deltaTime) * ratioScaled;
    }

    // Update is called once per frame
    void Update()
    {
        GenerateChunks();
        TickChunks();
    }

    void OnDestroy()
    {
        // Clean up any leftover tasks
        foreach (ChunkContainer chunkContainer in chunkFeelerNodeJobs.Keys)
        {
            (JobHandle handle, FeelerNodeSetJob job) = chunkFeelerNodeJobs[chunkContainer];
            handle.Complete();

            // Clean up
            job.Target.Dispose();
        }
    }
}