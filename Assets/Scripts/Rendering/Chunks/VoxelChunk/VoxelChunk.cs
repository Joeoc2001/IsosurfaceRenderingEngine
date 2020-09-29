using Algebra;
using AlgebraExtensions;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace SDFRendering.Chunks.VoxelChunk
{
    [RequireComponent(typeof(MeshFilter))]
    public class VoxelChunk : Chunk
    {
        private Mesh _mesh;

        void Awake()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        // Returns true if the mesh changed
        public void GenerateMesh(FeelerNodeSet nodes, Vector3Expression norm)
        {
            Voxeliser.Instance.MarchIntoMesh(_mesh, nodes);

            // Extract for speed
            Vector3[] vertices = _mesh.vertices;
            int[] triangles = _mesh.triangles;

            // Calculate normals
            if (ShouldApproximateNormals)
            {
                _mesh.normals = ApproximateNormals(vertices, triangles);
            }
            else
            {
                _mesh.normals = CalculateNormals(vertices, transform.position, norm);
            }
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new VoxelGenTask(this, sdf);
        }
    }
}