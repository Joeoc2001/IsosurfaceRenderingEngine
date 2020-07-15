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

    public override void Init(ChunkSet set, ChunkSystem system)
    {
        set.OnChunkRemoved += Set_OnChunkRemoved;
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

        // Create new feeler node update job
        PriorGenTask feelerNodeSetJob = chunk.CreatePriorJob(sdf);
        IPriorGenTaskHandle handle = feelerNodeSetJob.Schedule();
        priorGenJobs.Add(index, (handle, feelerNodeSetJob));
    }

    public override void Tick(ChunkSet set, ChunkSystem system)
    {
        FinishPriorGenJobs();

        // Calculate functions
        Equation distEq = distanceField.GetEquation();
        SDF sdf = new SDF(distEq);

        // Calculate how many chunks to tick
        int updates = (int)(set.Count * Time.deltaTime * updatesPerChunkSecond);

        // Loop through some and update
        HashSet<Vector3Int> updatedThisFrame = new HashSet<Vector3Int>();
        for (int i = 0; i < updates; i++)
        {
            Vector3Int index = system.RandomIndexSample();

            if (updatedThisFrame.Contains(index))
            {
                continue;
            }
            updatedThisFrame.Add(index);

            RandomTickChunk(set, system, index, sdf);
        }
    }
}
