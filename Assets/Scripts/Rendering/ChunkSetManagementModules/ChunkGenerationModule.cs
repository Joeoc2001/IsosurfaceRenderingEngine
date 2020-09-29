using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDFRendering.ChunkSetManagementModules
{
    public class ChunkGenerationModule : ChunkSetManagementModule
    {
        private Vector3Int generatedBaseIndex = new Vector3Int(0, 0, 0);
        private int generatedViewDistance = 0;

        public override void Tick(ChunkSet set, ChunkSystem system)
        {
            GenerateNewChunks(set, system);
        }

        public void GenerateNewChunks(ChunkSet chunkSet, ChunkSystem chunkSystem)
        {
            int viewDistance = chunkSystem.ViewDistance;
            Vector3Int baseIndex = chunkSet.BaseIndex;

            IEnumerable<Vector3Int> newChunkIndexes =
                chunkSystem.GetNewChunkIndexes(generatedBaseIndex, generatedViewDistance, baseIndex, viewDistance);

            foreach (Vector3Int index in newChunkIndexes)
            {
                if (!chunkSet.HasChunk(index))
                {
                    Chunk newChunk = chunkSystem.InstantiateNewChunk(chunkSet, index);
                    chunkSet.SetChunk(index, newChunk);
                }
            }

            generatedBaseIndex = baseIndex;
            generatedViewDistance = viewDistance;
        }
    }
}