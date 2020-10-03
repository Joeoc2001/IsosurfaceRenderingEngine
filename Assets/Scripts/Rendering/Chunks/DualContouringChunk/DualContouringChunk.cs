using Algebra;
using AlgebraExtensions;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace SDFRendering.Chunks.SurfaceNetChunk
{
    [RequireComponent(typeof(MeshFilter))]
    public class DualContouringChunk : MeshChunk
    {
        protected override void GeneratePolygonsIntoMesh(Mesh mesh, PointCloud nodes, ImplicitSurface surface)
        {
            DualContouringGenerator.Instance.MarchIntoMesh(mesh, nodes, surface);
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new DualContouringGenTask(this, sdf);
        }
    }
}