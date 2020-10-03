using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Algebra;
using System.Runtime.InteropServices;

namespace SDFRendering.Chunks.CubeMarchedChunk
{
    public class CubeMarchGenTask : GenTask
    {
        private readonly CubeMarchedChunk _chunk;
        private readonly ImplicitSurface _sdf;

        public CubeMarchGenTask(CubeMarchedChunk chunk, ImplicitSurface sdf)
        {
            this._chunk = chunk;
            this._sdf = sdf;
        }

        public override IPriorGenTaskHandle Schedule()
        {
            float delta = Chunk.SIZE / (1 << _chunk.Quality);
            int resolution = (1 << _chunk.Quality) + 1;
            Vector3 offset = _chunk.SamplingOffset;

            Sampler.SampleGridAsync(_sdf.Expression, AfterFinished, resolution, delta, offset);

            return new NoneTaskHandle();
        }

        public void AfterFinished(PointCloud nodes)
        {
            // Update chunk's mesh
            _chunk.GenerateMesh(nodes, _sdf);
        }
    }
}