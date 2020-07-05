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

    public EquationProvider distanceField;
    public ChunkManagementSystem chunkSystem;

    public int lastUpdates = 0;
    public double updatesPerSecond = 0;

    [Range(0, 2)]
    public double updatesPerChunkSecond = 0.1;

    [Range(0, 10000)]
    public int maxUpdatesPerSecond = 5000;

    [Range(1, 50)]
    public int viewDistance;

    public Vector3Int BaseIndex { get; set; }

    private TwoWayDict<Vector3Int, Chunk> chunks;
    private Dictionary<Chunk, ChunkContainer> containers;

    private Priority_Queue.FastPriorityQueue<ChunkContainer> updatePriority;

    public Vector3Int? GetChunkIndex(Chunk chunk)
    {
        if(chunks.TryGetValue(chunk, out Vector3Int index))
        {
            return index;
        }

        return null;
    }

    public Vector3Int? GetChunkOffset(Chunk chunk)
    {
        Vector3Int? indexQuery = GetChunkIndex(chunk);
        if (!indexQuery.HasValue)
        {
            return null;
        }

        Vector3Int index = indexQuery.Value;
        return index - BaseIndex;
    }

    // Start is called before the first frame update
    void Start()
    {
        chunks = new TwoWayDict<Vector3Int, Chunk>();
        containers = new Dictionary<Chunk, ChunkContainer>();
        updatePriority = new Priority_Queue.FastPriorityQueue<ChunkContainer>(GetMaxChunks());
    }

    private int GetMaxChunks()
    {
        int side = (2 * viewDistance) + 1;
        return side * side * side;
    }

    public bool ShouldChunkBeKilled(Chunk chunk)
    {
        Vector3Int? offsetQuery = GetChunkOffset(chunk);
        if (!offsetQuery.HasValue)
        {
            return false; // If the chunk isn't present then it probably shouldn't be killed
        }

        Vector3Int offset = offsetQuery.Value;
        int dist = Math.Max(Math.Max(offset.x, offset.y), offset.z);
        return dist > viewDistance + 1;
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

    public void UpdateChunkFields(Chunk chunk)
    {
        Vector3Int? indexQuery = GetChunkIndex(chunk);
        if (!indexQuery.HasValue)
        {
            return;
        }

        Vector3Int index = indexQuery.Value;
        Vector3 position = new Vector3(index.x, index.y, index.z) * Chunk.SIZE;
        chunk.transform.localPosition = position;
        chunk.OwningSet = this;
        chunk.SetQuality(chunkSystem.GetChunkQuality(this, chunk));
    }

    public Chunk GenerateChunk(Vector3Int index)
    {
        // Generate a new chunk
        Chunk newChunk = chunkSystem.InstantiateNewChunk(this, index);

        // Add new chunk to data structures
        ChunkContainer container = new ChunkContainer(newChunk);
        containers.Add(newChunk, container);
        updatePriority.Enqueue(container, container.Priority);
        chunks.Add(index, newChunk);

        // Set fields
        UpdateChunkFields(newChunk);

        return newChunk;
    }

    public void RemoveChunk(Vector3Int index)
    {
        // If the index isn't in chunks then the chunk doesn't exist
        if (!chunks.TryGetValue(index, out Chunk chunk))
        {
            return;
        }

        chunk.OwningSet = null;

        // Remove the chunk from the list of chunks to be ticked (if it is present)
        ChunkContainer container = containers[chunk];
        if(updatePriority.Contains(container))
        {
            updatePriority.Remove(container);
        }

        // Remove the chunk from the dictionaries
        containers.Remove(chunk);
        chunks.Remove(index);

        // Kill it
        Destroy(chunk.gameObject);
    }

    private Vector3Int generatedBaseIndex = new Vector3Int(0, 0, 0);
    private int generatedViewDistance = 0;
    public void GenerateNewChunks()
    {
        IEnumerable<Vector3Int> newChunkIndexes = GetNewChunkIndexes(generatedBaseIndex, generatedViewDistance, BaseIndex, viewDistance);

        // Check if priority queue needs resizing
        if (viewDistance > generatedViewDistance || viewDistance < generatedViewDistance / 1.2)
        {
            updatePriority.Resize(GetMaxChunks());
        }

        foreach (Vector3Int index in newChunkIndexes)
        {
            if (!chunks.ContainsKey(index))
            {
                GenerateChunk(index);
            }

            Assert.IsNotNull(chunks[index]);
        }

        generatedBaseIndex = BaseIndex;
        generatedViewDistance = viewDistance;
    }

    private readonly Dictionary<ChunkContainer, (IPriorGenTaskHandle handle, PriorGenTask job)> priorGenJobs
        = new Dictionary<ChunkContainer, (IPriorGenTaskHandle handle, PriorGenTask job)>();

    private List<ChunkContainer> FinishPriorGenJobs()
    {
        List<ChunkContainer> completedJobs = new List<ChunkContainer>(priorGenJobs.Count);

        foreach (ChunkContainer chunkContainer in priorGenJobs.Keys)
        {
            (IPriorGenTaskHandle handle, PriorGenTask job) = priorGenJobs[chunkContainer];

            // Run job to completion
            handle.Complete();

            // Update Chunk
            job.AfterFinished();

            // Add chunk to be modified
            completedJobs.Add(chunkContainer);
        }

        priorGenJobs.Clear();

        return completedJobs;
    }

    private void RandomTickChunk(ChunkContainer chunkContainer, SDF sdf)
    {
        Chunk chunk = chunkContainer.Chunk;
        Vector3Int index = chunks[chunk];

        // Check to see if the chunk should be deleted
        if (ShouldChunkBeKilled(chunk))
        {
            RemoveChunk(index);
            return;
        }

        // Check to see if the chunk should be changed
        if (chunkSystem.ShouldChunkBeReinstantiated(chunk))
        {
            chunk = GenerateChunk(index);
        }

        // Update fields
        UpdateChunkFields(chunk);

        // Create new feeler node update job
        PriorGenTask feelerNodeSetJob = chunk.CreatePriorJob(sdf);
        IPriorGenTaskHandle handle = feelerNodeSetJob.Schedule();
        priorGenJobs.Add(chunkContainer, (handle, feelerNodeSetJob));
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
        SDF sdf = new SDF(distEq);

        // Complete all prior gen node tasks
        List<ChunkContainer> chunkContainers = FinishPriorGenJobs();
        foreach (ChunkContainer chunkContainer in chunkContainers)
        {
            // Requeue chunk
            chunkContainer.SetLastUpdatedTime();
            updatePriority.Enqueue(chunkContainer, chunkContainer.Priority);
        }

        int updates = GetNumberOfChunksToTick();
        // Loop through some and update
        for (int i = 0; i < updates; i++)
        {
            ChunkContainer chunkContainer = updatePriority.Dequeue();
            RandomTickChunk(chunkContainer, sdf);
        }

        UpdateMetrics(updates);
    }

    // Update is called once per frame
    void Update()
    {
        GenerateNewChunks(); // Checks which chunks should exist given the position etc of this set
        TickChunks(); // Updates a collection of the chunks
    }

    void OnDestroy()
    {
        // Clean up any leftover tasks
        FinishPriorGenJobs();
    }
}