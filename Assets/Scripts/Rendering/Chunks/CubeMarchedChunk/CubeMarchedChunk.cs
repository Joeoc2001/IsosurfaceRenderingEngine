using Algebra;
using AlgebraExtensions;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace SDFRendering.Chunks.CubeMarchedChunk
{
    [RequireComponent(typeof(MeshFilter))]
    public class CubeMarchedChunk : MeshChunk
    {
        protected override (Vector3[], int[]) GeneratePolygons(PointCloud nodes, ImplicitSurface surface)
        {
            return CubeMarcher.Instance.MarchIntoMesh(nodes, surface);
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new CubeMarchGenTask(this, sdf);
        }
    }
}