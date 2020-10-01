using Algebra;
using AlgebraExtensions;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace SDFRendering.Chunks.SurfaceNetChunk
{
    [RequireComponent(typeof(MeshFilter))]
    public class DualContouringChunk : Chunk
    {
        private Mesh _mesh;

        void Awake()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        // Returns true if the mesh changed
        public void GenerateMesh(FeelerNodeSet nodes, ImplicitSurface surface)
        {
            DualContouringGenerator.Instance.MarchIntoMesh(_mesh, nodes, surface);

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
                _mesh.normals = CalculateNormals(vertices, transform.position, surface.Gradient);
            }
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new DualContouringGenTask(this, sdf);
        }
    }
}