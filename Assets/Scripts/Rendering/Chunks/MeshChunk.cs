using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SDFRendering.Chunks
{
    [RequireComponent(typeof(MeshFilter))]
    public abstract class MeshChunk : Chunk
    {
        private Mesh _mesh;

        void Awake()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        protected abstract void GeneratePolygonsIntoMesh(Mesh mesh, PointCloud nodes, ImplicitSurface surface);

        public void GenerateMesh(PointCloud nodes, ImplicitSurface surface)
        {
            GeneratePolygonsIntoMesh(_mesh, nodes, surface);

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

            Dirty = false;
        }
    }
}
