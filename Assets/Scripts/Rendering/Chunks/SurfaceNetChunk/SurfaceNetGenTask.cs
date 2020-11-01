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
using System.Threading.Tasks;

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

        public override ITaskHandle Schedule()
        {
            int sideResolution = (1 << _chunk.Quality) + 2;
            float sideSpacing = Chunk.SIZE / (1 << _chunk.Quality);
            Vector3 origin = _chunk.SamplingOffset - Vector3.one * (sideSpacing / 2);

            Task task = Task.Run(() => AfterFinished(Sampler.SampleGridAsync(_sdf.Expression, sideResolution, sideSpacing, origin)));

            return new TaskTaskHandle(task);
        }

        public void AfterFinished(PointCloud nodes)
        {
            // Update chunk's mesh
            _chunk.GenerateMesh(nodes, _sdf);
        }
    }
}