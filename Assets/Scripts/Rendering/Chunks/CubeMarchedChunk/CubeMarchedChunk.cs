using Algebra;
using AlgebraExtensions;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace SDFRendering.Chunks.CubeMarchedChunk
{
    [RequireComponent(typeof(MeshFilter))]
    public class CubeMarchedChunk : Chunk
    {
        private Mesh _coreMesh;
        private Mesh _transitionMesh;

        private Mesh _totalMesh;

        void Awake()
        {
            _coreMesh = new Mesh();
            _transitionMesh = new Mesh();

            _totalMesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _totalMesh;
        }

        // Returns true if the mesh changed
        public void GenerateMesh(FeelerNodeSet nodes, ImplicitSurface surface)
        {
            CubeMarcher.Instance.MarchIntoMesh(_coreMesh, nodes, surface);

            // Extract for speed
            Vector3[] vertices = _coreMesh.vertices;
            int[] triangles = _coreMesh.triangles;

            // Calculate normals
            if (ShouldApproximateNormals)
            {
                _coreMesh.normals = ApproximateNormals(vertices, triangles);
            }
            else
            {
                _coreMesh.normals = CalculateNormals(vertices, transform.position, surface.Gradient);
            }
        }

        public void MergeAndSetMeshes()
        {
            _totalMesh.Clear();

            CombineInstance[] instances = new CombineInstance[] {
            new CombineInstance() {
                mesh = _coreMesh
            },
            new CombineInstance() {
                mesh = _transitionMesh
            },
        };

            _totalMesh.CombineMeshes(instances, true, false, false);
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new CubeMarchGenTask(this, sdf);
        }
    }
}