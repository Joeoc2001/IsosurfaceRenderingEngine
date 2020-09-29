using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDFRendering.ChunkSetManagementModules
{
    public class ChunkGenerationModule : ChunkSetManagementModule
    {
        private Vector3Int _generatedBaseIndex = new Vector3Int(0, 0, 0);
        private int _generatedViewDistance = 0;

        public override void Tick(ChunkSet set, ChunkSystem system)
        {
            GenerateNewChunks(set, system);
        }

        public void GenerateNewChunks(ChunkSet chunkSet, ChunkSystem chunkSystem)
        {
            int viewDistance = chunkSystem.ViewDistance;
            Vector3Int baseIndex = chunkSet.BaseIndex;

            IEnumerable<Vector3Int> newChunkIndexes =
                chunkSystem.GetNewChunkIndexes(_generatedBaseIndex, _generatedViewDistance, baseIndex, viewDistance);

            foreach (Vector3Int index in newChunkIndexes)
            {
                if (!chunkSet.HasChunk(index))
                {
                    Chunk newChunk = chunkSystem.InstantiateNewChunk(chunkSet, index);
                    chunkSet.SetChunk(index, newChunk);
                }
            }

            _generatedBaseIndex = baseIndex;
            _generatedViewDistance = viewDistance;
        }
    }
}