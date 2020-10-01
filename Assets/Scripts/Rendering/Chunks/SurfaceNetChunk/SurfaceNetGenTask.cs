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

namespace SDFRendering.Chunks.SurfaceNetChunk
{
    public class SurfaceNetGenTask : GenTask
    {
        private readonly SurfaceNetChunk _chunk;
        private readonly ImplicitSurface _sdf;

        public SurfaceNetGenTask(SurfaceNetChunk chunk, ImplicitSurface sdf)
        {
            this._chunk = chunk;
            this._sdf = sdf;
        }

        public override IPriorGenTaskHandle Schedule()
        {
            int resolution = (1 << _chunk.Quality) + 2;
            float delta = Chunk.SIZE / (1 << _chunk.Quality);
            Vector3 offset = new Vector3(-1, -1, -1) * (delta / 2);
            Vector3 samplingOffset = _chunk.transform.position;

            Sampler.SampleGridAsync(_sdf.Expression, AfterFinished, resolution, delta, offset, samplingOffset);

            return new NoneTaskHandle();
        }

        public void AfterFinished(FeelerNodeSet nodes)
        {
            // Update chunk's mesh
            _chunk.GenerateMesh(nodes, _sdf.Gradient);
        }
    }
}