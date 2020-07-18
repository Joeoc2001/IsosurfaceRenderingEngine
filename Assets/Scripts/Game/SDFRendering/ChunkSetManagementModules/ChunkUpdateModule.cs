using Algebra;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkUpdateModule : ChunkSetManagementModule
{
    public EquationProvider distanceField;

    [Range(0, 2)]
    public double updatesPerChunkSecond = 0.1;

    [Range(0, 1000)]
    public int maxUpdatesPerTick = 100;

    private readonly Dictionary<Vector3Int, (IPriorGenTaskHandle handle, PriorGenTask job)> priorGenJobs
        = new Dictionary<Vector3Int, (IPriorGenTaskHandle handle, PriorGenTask job)>();

    private SDF lastSDF = null;

    public override void Init(ChunkSet set, ChunkSystem system)
    {
        // Also caputure system. Probably won't cause a memory leak beacause these objects should always exist
        set.OnChunkAdded += (s, c, i) => Set_OnChunkAdded(s, system, c, i);
        set.OnChunkRemoved += Set_OnChunkRemoved;
    }

    private SDF genSDF()
    {
        // Calculate functions
        lastSDF = new SDF(distanceField.GetEquation());
        return lastSDF;
    }

    private SDF getLastSDF()
    {
        if (lastSDF == null)
        {
            return genSDF();
        }
        return lastSDF;
    }

    private void Set_OnChunkAdded(ChunkSet set, ChunkSystem system, Chunk chunk, Vector3Int index)
    {
        RandomTickChunk(set, system, index, getLastSDF());
    }

    private void Set_OnChunkRemoved(ChunkSet set, Chunk chunk, Vector3Int index)
    {
        if (priorGenJobs.ContainsKey(index))
        {
            priorGenJobs[index].handle.Complete(); // Ensure no dangling jobs
            priorGenJobs.Remove(index);
        }
    }

    public override void Destruct(ChunkSet set, ChunkSystem system)
    {
        FinishPriorGenJobs(); // Ensure that all jobs are done
    }

    private List<Vector3Int> FinishPriorGenJobs()
    {
        List<Vector3Int> completedJobs = new List<Vector3Int>(priorGenJobs.Count);

        foreach (Vector3Int index in priorGenJobs.Keys)
        {
            (IPriorGenTaskHandle handle, PriorGenTask job) = priorGenJobs[index];

            // Run job to completion
            handle.Complete();

            // Update Chunk
            job.AfterFinished();

            // Add chunk to be modified
            completedJobs.Add(index);
        }

        priorGenJobs.Clear();

        return completedJobs;
    }

    private void RandomTickChunk(ChunkSet set, ChunkSystem system, Vector3Int index, SDF sdf)
    {
        // Check to see if the chunk should be changed
        if (!set.TryGetChunk(index, out Chunk chunk) || system.ShouldChunkBeReinstantiated(chunk))
        {
            // Create a new chunk
            chunk = system.InstantiateNewChunk(set, index);

            // Add it to the set
            set.SetChunk(index, chunk);
        }

        // Update the chunk's quality
        chunk.Quality = system.GetChunkQuality(set, chunk);

        // Create new feeler node update job
        PriorGenTask feelerNodeSetJob = chunk.CreatePriorJob(sdf);
        IPriorGenTaskHandle handle = feelerNodeSetJob.Schedule();
        priorGenJobs.Add(index, (handle, feelerNodeSetJob));
    }

    public override void Tick(ChunkSet set, ChunkSystem system)
    {
        FinishPriorGenJobs();

        // Calculate functions
        SDF sdf = genSDF();

        // Calculate how many chunks to tick
        int updates = (int)(set.Count * Time.deltaTime * updatesPerChunkSecond);

        // Loop through some and update
        HashSet<Vector3Int> updatedThisFrame = new HashSet<Vector3Int>();
        for (int i = 0; i < updates; i++)
        {
            Vector3Int index = system.RandomIndexSample();

            if (updatedThisFrame.Contains(index))
            {
                i--;
                continue;
            }
            updatedThisFrame.Add(index);

            RandomTickChunk(set, system, index, sdf);
        }
    }
}
