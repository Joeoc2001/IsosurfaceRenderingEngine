using Algebra;
using AlgebraExtensions;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace SDFRendering.Chunks.VoxelChunk
{
    [RequireComponent(typeof(MeshFilter))]
    public class VoxelChunk : MeshChunk
    {
        protected override void GeneratePolygonsIntoMesh(Mesh mesh, FeelerNodeSet nodes, ImplicitSurface surface)
        {
            Voxeliser.Instance.MarchIntoMesh(mesh, nodes, surface);
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new VoxelGenTask(this, sdf);
        }
    }
}