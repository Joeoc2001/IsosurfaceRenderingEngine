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
    private class ChunkPriority : IComparable<ChunkPriority>
    {
        private readonly double lastUpdatedTime;
        private readonly bool isEmpty;

        public ChunkPriority(double lastUpdatedTime, bool isEmpty)
        {
            this.lastUpdatedTime = lastUpdatedTime;
            this.isEmpty = isEmpty;
        }

        public double PriorityFunction()
        {
            return lastUpdatedTime;
        }

        public int CompareTo(ChunkPriority other)
        {
            return PriorityFunction().CompareTo(other.PriorityFunction());
        }
    }

    public Chunk baseChunk;

    public EquationProvider distanceField;

    public int lastUpdates = 0;

    [Range(0, 2)]
    public double updatesPerChunkSecond = 0.1;

    [Range(0, 100)]
    public int maxUpdates = 10;

    [Range(1, 50)]
    public int viewDistance;

    [Range(0, 50)]
    public float normalApproxDistance;

    [Range(0, 2)]
    public float qualityDropoff;

    public Vector3Int baseIndex;

    private TwoWayDict<Vector3Int, Chunk> chunks;

    private Priority_Queue.SimplePriorityQueue<Chunk, ChunkPriority> updatePriority;

    // Start is called before the first frame update
    void Start()
    {
        chunks = new TwoWayDict<Vector3Int, Chunk>();
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

                updatePriority.Enqueue(chunk, new ChunkPriority(0, false));

                chunks.Add(index, chunk);
            }
            //Assert.IsNotNull(chunks[index]);
        }

        generatedBaseIndex = baseIndex;
        generatedViewDistance = viewDistance;
    }

    private readonly Dictionary<Chunk, (JobHandle handle, FeelerNodeSetJob job)> chunkFeelerNodeJobs
        = new Dictionary<Chunk, (JobHandle handle, FeelerNodeSetJob job)>();

    public void TickChunks()
    {
        // Calculate functions
        Equation distEq = distanceField.GetEquation().GetSimplified();
        Equation.ExpressionDelegate distFunc = distEq.GetExpression();
        Equation.ExpressionDelegate dxFunc = distEq.GetDerivative(Variable.X).GetSimplified().GetExpression();
        Equation.ExpressionDelegate dyFunc = distEq.GetDerivative(Variable.Y).GetSimplified().GetExpression();
        Equation.ExpressionDelegate dzFunc = distEq.GetDerivative(Variable.Z).GetSimplified().GetExpression();
        Func<VariableSet, Vector3> normFunc = v => new Vector3(dxFunc(v), dyFunc(v), dzFunc(v));

        // Find complete all feeler node tasks
        List<Chunk> jobChunks = chunkFeelerNodeJobs.Keys.ToList();
        foreach (Chunk chunk in jobChunks)
        {
            (JobHandle handle, FeelerNodeSetJob job) = chunkFeelerNodeJobs[chunk];
            handle.Complete();

            // Extract completed datas
            FeelerNode[] feelerNodes = job.Target.ToArray();
            int resolution = job.Resolution;
            FeelerNodeSet feelerNodeSet = new FeelerNodeSet(resolution, feelerNodes);

            // Update chunk
            chunk.GenerateMesh(feelerNodeSet, normFunc);

            // Remove the chunk from the jobs dict and add it back to the queue
            chunkFeelerNodeJobs.Remove(chunk);

            // Clean up
            job.Target.Dispose();

            updatePriority.Enqueue(chunk, new ChunkPriority(Time.time, chunk.IsEmpty));
        }

        // Loop through some and update
        int updates = (int)(updatesPerChunkSecond * Time.deltaTime * updatePriority.Count);
        updates = Math.Min(updates, updatePriority.Count);
        updates = Math.Min(updates, maxUpdates);
        for (int i = 0; i < updates; i++)
        {
            Chunk chunk = updatePriority.Dequeue();

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
            chunkFeelerNodeJobs.Add(chunk, (handle, feelerNodeSetJob));
        }
        lastUpdates = updates;
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
        foreach (Chunk chunk in chunkFeelerNodeJobs.Keys)
        {
            (JobHandle handle, FeelerNodeSetJob job) = chunkFeelerNodeJobs[chunk];
            handle.Complete();

            // Clean up
            job.Target.Dispose();
        }
    }
}