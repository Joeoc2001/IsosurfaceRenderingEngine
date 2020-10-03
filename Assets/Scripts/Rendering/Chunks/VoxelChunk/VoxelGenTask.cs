using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Algebra;
using System.Runtime.InteropServices;
using SDFRendering.JobHandles;

namespace SDFRendering.Chunks.VoxelChunk
{
    public class VoxelGenTask : GenTask
    {
        private readonly VoxelChunk _chunk;
        private readonly ImplicitSurface _sdf;

        public VoxelGenTask(VoxelChunk chunk, ImplicitSurface sdf)
        {
            this._chunk = chunk;
            this._sdf = sdf;
        }

        public override IPriorGenTaskHandle Schedule()
        {
            int sideResolution = (1 << _chunk.Quality) + 2;
            float sideSpacing = Chunk.SIZE / (1 << _chunk.Quality);
            Vector3 origin = _chunk.SamplingOffset - Vector3.one * (sideSpacing / 2);

            Sampler.SampleGridAsync(_sdf.Expression, AfterFinished, sideResolution, sideSpacing, origin);

            return new NoneTaskHandle();
        }

        public void AfterFinished(PointCloud nodes)
        {
            // Update chunk's mesh
            _chunk.GenerateMesh(nodes, _sdf);
        }
    }
}