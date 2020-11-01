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

        [Range(0.0f, 0.016f)]
        public float maxTimePerFrame = 0.008f;

        [Range(1, 16)]
        public int maxJobs = 4;

        private readonly Dictionary<Vector3Int, ITaskHandle> _genJobs = new Dictionary<Vector3Int, ITaskHandle>();

        public override void Init(ChunkSet set, ChunkSystem system)
        {
            set.OnChunkRemoved += Set_OnChunkRemoved;
        }

        private ImplicitSurface GenSDF()
        {
            // Calculate functions
            return new ImplicitSurface(distanceField.GetExpression());
        }

        private void Set_OnChunkRemoved(ChunkSet set, Chunk chunk, Vector3Int index)
        {
            if (_genJobs.ContainsKey(index))
            {
                _genJobs[index].Complete(); // Ensure no dangling jobs
                _genJobs.Remove(index);
            }
        }

        public override void Destruct(ChunkSet set, ChunkSystem system)
        {
            FinishAllJobs(); // Ensure that all jobs are done
        }

        private void FinishAllJobs()
        {
            foreach (var item in _genJobs)
            {
                ITaskHandle handle = item.Value;

                // Finish and collect job
                handle.Complete();
            }

            _genJobs.Clear();
        }

        private List<Vector3Int> CollectFinishedJobs()
        {
            List<Vector3Int> completed = new List<Vector3Int>();

            foreach (var item in _genJobs)
            {
                Vector3Int index = item.Key;
                ITaskHandle handle = item.Value;

                if (!handle.HasFinished())
                {
                    continue;
                }

                // Finish and collect job
                handle.Complete();

                // Add chunk to be removed
                completed.Add(index);
            }

            foreach (Vector3Int index in completed)
            {
                _genJobs.Remove(index);
            }

            return completed;
        }

        private void UpdateChunk(ChunkSet set, ChunkSystem system, Chunk chunk, ImplicitSurface sdf)
        {
            Vector3Int index = set.GetChunkIndex(chunk).Value;

            if (system.ShouldChunkBeReinstantiated(chunk))
            {
                chunk = system.InstantiateNewChunk(set, index);
                set.SetChunk(index, chunk);
            }

            chunk.Quality = system.GetChunkQuality(set, chunk);

            // Create new feeler node update job
            GenTask genTask = chunk.CreateGetTask(sdf);
            ITaskHandle handle = genTask.Schedule();
            _genJobs.Add(index, handle);
        }

        private static IEnumerable<T> Shuffle<T>(IEnumerable<T> enumerable)
        {
            List<T> ts = new List<T>(enumerable);
            int n = ts.Count;
            while (n > 1)
            {
                int k = UnityEngine.Random.Range(0, n);
                n -= 1;
                T temp = ts[n];
                ts[n] = ts[k];
                ts[k] = temp;
            }
            return ts;
        }

        public override void Tick(ChunkSet set, ChunkSystem system)
        {
            CollectFinishedJobs();

            ImplicitSurface sdf = GenSDF();

            float startTime = Time.realtimeSinceStartup;

            // Loop through some and update
            foreach (Chunk chunk in Shuffle(set))
            {
                if (_genJobs.Count >= maxJobs)
                {
                    break;
                }

                float nowTime = Time.realtimeSinceStartup;
                if (nowTime - startTime > maxTimePerFrame)
                {
                    break;
                }

                Vector3Int index = set.GetChunkIndex(chunk).Value;

                if (!chunk.Dirty || _genJobs.ContainsKey(index))
                {
                    continue;
                }

                UpdateChunk(set, system, chunk, sdf);
            }
        }
    }
}