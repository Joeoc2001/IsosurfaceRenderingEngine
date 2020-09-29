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
        private Mesh mesh;

        void Awake()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        // Returns true if the mesh changed
        public void GenerateMesh(FeelerNodeSet nodes, Vector3Expression norm)
        {
            Voxeliser.Instance.MarchIntoMesh(mesh, nodes);

            // Extract for speed
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            // Calculate normals
            if (ShouldApproximateNormals)
            {
                mesh.normals = ApproximateNormals(vertices, triangles);
            }
            else
            {
                mesh.normals = CalculateNormals(vertices, transform.position, norm);
            }
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new VoxelGenTask(this, sdf);
        }
    }
}