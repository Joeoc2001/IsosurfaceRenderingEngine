using Algebra;
using AlgebraExtensions;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace SDFRendering.Chunks.SurfaceNetChunk
{
    [RequireComponent(typeof(MeshFilter))]
    public class SurfaceNetChunk : MeshChunk
    {
        protected override void GeneratePolygonsIntoMesh(Mesh mesh, FeelerNodeSet nodes, ImplicitSurface surface)
        {
            SurfaceNetGenerator.Instance.MarchIntoMesh(mesh, nodes, surface);
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new SurfaceNetGenTask(this, sdf);
        }
    }
}