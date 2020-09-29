using Algebra;
using AlgebraExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SDFRendering.ChunkSetManagementModules
{
    public class ChunkUpdateModule : ChunkSetManagementModule
    {
        public ExpressionProvider distanceField;

        [Range(0, 2)]
        public double updatesPerChunkSecond = 0.1;

        [Range(0, 1000)]
        public int maxUpdatesPerTick = 100;

        private readonly Dictionary<Vector3Int, IPriorGenTaskHandle> genJobs = new Dictionary<Vector3Int, IPriorGenTaskHandle>();

        private ImplicitSurface lastSDF = null;

        public override void Init(ChunkSet set, ChunkSystem system)
        {
            // Also caputure system. Probably won't cause a memory leak beacause these objects should always exist
            set.OnChunkAdded += (s, c, i) => Set_OnChunkAdded(s, system, c, i);
            set.OnChunkRemoved += Set_OnChunkRemoved;
        }

        private ImplicitSurface genSDF()
        {
            // Calculate functions
            lastSDF = new ImplicitSurface(distanceField.GetExpression());
            return lastSDF;
        }

        private ImplicitSurface getLastSDF()
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
            if (genJobs.ContainsKey(index))
            {
                genJobs[index].Complete(); // Ensure no dangling jobs
                genJobs.Remove(index);
            }
        }

        public override void Destruct(ChunkSet set, ChunkSystem system)
        {
            FinishGenJobs(); // Ensure that all jobs are done
        }

        private List<Vector3Int> FinishGenJobs()
        {
            List<Vector3Int> completedJobs = new List<Vector3Int>(genJobs.Count);

            foreach (Vector3Int index in genJobs.Keys)
            {
                IPriorGenTaskHandle handle = genJobs[index];

                // Run job to completion
                handle.Complete();

                // Add chunk to be modified
                completedJobs.Add(index);
            }

            genJobs.Clear();

            return completedJobs;
        }

        private void RandomTickChunk(ChunkSet set, ChunkSystem system, Vector3Int index, ImplicitSurface sdf)
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
            GenTask genTask = chunk.CreateGetTask(sdf);
            IPriorGenTaskHandle handle = genTask.Schedule();
            genJobs.Add(index, handle);
        }

        public override void Tick(ChunkSet set, ChunkSystem system)
        {
            FinishGenJobs();

            // Calculate functions
            ImplicitSurface sdf = genSDF();

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
}