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
            int edgesPerEdge = 1 << _chunk.Quality;
            float delta = Chunk.SIZE / edgesPerEdge;
            int verticesPerEdge = edgesPerEdge + 1;
            Vector3 offset = new Vector3(-1, -1, -1) * (Chunk.SIZE / 2);
            Vector3 samplingOffset = _chunk.transform.position;

            Sampler.SampleGridAsync(_sdf.Expression, AfterFinished, verticesPerEdge, delta, offset, samplingOffset);

            return new NoneTaskHandle();
        }

        public void AfterFinished(FeelerNodeSet nodes)
        {
            // Update chunk's mesh
            _chunk.GenerateMesh(nodes, _sdf);

            // Set chunk meshes
            _chunk.MergeAndSetMeshes();
        }
    }
}