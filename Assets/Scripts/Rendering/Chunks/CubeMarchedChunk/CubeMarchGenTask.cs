using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Algebra;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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

        public override ITaskHandle Schedule()
        {
            float delta = Chunk.SIZE / (1 << _chunk.Quality);
            int resolution = (1 << _chunk.Quality) + 1;
            Vector3 offset = _chunk.SamplingOffset;

            Task task = Task.Run(() => AfterFinished(Sampler.SampleGridAsync(_sdf.Expression, resolution, delta, offset)));

            return new TaskTaskHandle(task);
        }

        public void AfterFinished(PointCloud nodes)
        {
            // Update chunk's mesh
            _chunk.GenerateMesh(nodes, _sdf);
        }
    }
}