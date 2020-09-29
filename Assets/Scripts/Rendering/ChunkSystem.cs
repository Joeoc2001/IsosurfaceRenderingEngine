using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDFRendering
{
    public abstract class ChunkSystem : MonoBehaviour
    {
        [Range(1, 50)]
        public int ViewDistance;

        public ChunkSetManagementModule[] modules;

        public ChunkSet ChunkSet;

        public abstract bool ShouldChunkBeReinstantiated(Chunk chunk);

        public abstract Chunk InstantiateNewChunk(ChunkSet destination, Vector3Int index);

        public abstract int GetChunkQuality(ChunkSet owner, Chunk chunk);

        public virtual bool ShouldChunkBeKilled(Vector3Int index)
        {
            Vector3Int offset = index - ChunkSet.BaseIndex;
            int dist = Math.Max(Math.Max(offset.x, offset.y), offset.z);
            return dist > ViewDistance;
        }

        public virtual IEnumerable<Vector3Int> GetNewChunkIndexes(Vector3Int oldCenter, int oldR, Vector3Int newCenter, int newR)
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

        public virtual Vector3Int RandomIndexSample()
        {
            int randomOffset()
            {
                return UnityEngine.Random.Range(-ViewDistance, ViewDistance);
            }
            Vector3Int offset = new Vector3Int(randomOffset(), randomOffset(), randomOffset());
            return offset + ChunkSet.BaseIndex;
        }

        void Awake()
        {
            foreach (ChunkSetManagementModule module in modules)
            {
                module.Init(ChunkSet, this);
            }
        }

        void Update()
        {
            foreach (ChunkSetManagementModule module in modules)
            {
                module.Tick(ChunkSet, this);
            }
        }

        void OnDestroy()
        {
            foreach (ChunkSetManagementModule module in modules)
            {
                module.Destruct(ChunkSet, this);
            }
        }
    }
}