using System;
using UnityEngine;

namespace SDFRendering.Chunks.VoxelChunk
{
    public class Voxeliser : DualGenerator
    {
        public static readonly Voxeliser Instance = new Voxeliser();

        private Voxeliser()
        {

        }

        protected override Vector3 CalculateVertex(PointCloud nodes, ImplicitSurface surface, Vector3Int index)
        {
            return (nodes[index].Pos + nodes[index + new Vector3Int(1, 1, 1)].Pos) / 2;
        }
    }
}
