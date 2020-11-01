using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace SDFRendering.Chunks
{
    [RequireComponent(typeof(MeshFilter))]
    public abstract class MeshChunk : Chunk
    {
        private Mesh _mesh;

        // Mutex for updating mesh on main thread
        private readonly Mutex _mut = new Mutex();
        private bool _meshNeedsUpdating = false;

        // Mesh components for updating
        private Vector3[] _newVertices;
        private int[] _newTriangles;
        private Vector3[] _newNormals;

        void Awake()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        void Update()
        {
            if (_meshNeedsUpdating && _mut.WaitOne(1))
            {
                _mesh.Clear();
                _mesh.SetVertices(_newVertices);
                _mesh.SetTriangles(_newTriangles, 0);
                _mesh.SetNormals(_newNormals);

                _newVertices = null;
                _newTriangles = null;
                _newNormals = null;

                _meshNeedsUpdating = false;

                _mut.ReleaseMutex();
            }
        }

        protected abstract (Vector3[], int[]) GeneratePolygons(PointCloud nodes, ImplicitSurface surface);

        public void GenerateMesh(PointCloud nodes, ImplicitSurface surface)
        {
            (Vector3[] newVertices, int[] newTriangles) = GeneratePolygons(nodes, surface);

            // Calculate normals
            Vector3[] newNormals;
            if (ShouldApproximateNormals)
            {
                newNormals = ApproximateNormals(newVertices, newTriangles);
            }
            else
            {
                newNormals = CalculateNormals(newVertices, SamplingOffset, surface.Gradient);
            }

            Dirty = false;

            _mut.WaitOne();
            {
                _meshNeedsUpdating = true;

                _newVertices = newVertices;
                _newTriangles = newTriangles;
                _newNormals = newNormals;
            }
            _mut.ReleaseMutex();
        }
    }
}
