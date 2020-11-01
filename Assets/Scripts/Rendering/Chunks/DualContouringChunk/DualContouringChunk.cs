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
        protected override (Vector3[], int[]) GeneratePolygons(PointCloud nodes, ImplicitSurface surface)
        {
            return DualContouringGenerator.Instance.MarchIntoMesh(nodes, surface);
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new DualContouringGenTask(this, sdf);
        }
    }
}