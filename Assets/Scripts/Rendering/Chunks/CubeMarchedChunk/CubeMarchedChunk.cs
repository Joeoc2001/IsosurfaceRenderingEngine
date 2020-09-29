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
        private Mesh coreMesh;
        private Mesh transitionMesh;

        private Mesh totalMesh;

        void Awake()
        {
            coreMesh = new Mesh();
            transitionMesh = new Mesh();

            totalMesh = new Mesh();
            GetComponent<MeshFilter>().mesh = totalMesh;
        }

        // Returns true if the mesh changed
        public void GenerateMesh(FeelerNodeSet nodes, Vector3Expression norm)
        {
            CubeMarcher.Instance.MarchIntoMesh(coreMesh, nodes);

            // Extract for speed
            Vector3[] vertices = coreMesh.vertices;
            int[] triangles = coreMesh.triangles;

            // Calculate normals
            if (ShouldApproximateNormals)
            {
                coreMesh.normals = ApproximateNormals(vertices, triangles);
            }
            else
            {
                coreMesh.normals = CalculateNormals(vertices, transform.position, norm);
            }
        }

        public void MergeAndSetMeshes()
        {
            totalMesh.Clear();

            CombineInstance[] instances = new CombineInstance[] {
            new CombineInstance() {
                mesh = coreMesh
            },
            new CombineInstance() {
                mesh = transitionMesh
            },
        };

            totalMesh.CombineMeshes(instances, true, false, false);
        }

        public override GenTask CreateGetTask(ImplicitSurface sdf)
        {
            return new CubeMarchGenTask(this, sdf);
        }
    }
}