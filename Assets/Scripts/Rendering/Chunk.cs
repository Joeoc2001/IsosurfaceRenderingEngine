using Algebra;
using AlgebraExtensions;
using AlgebraUnityExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDFRendering
{
    public abstract class Chunk : MonoBehaviour
    {
        public static readonly int APPROXIMATE_NORMALS_QUALITY = 2;

        [Range(1, 8)]
        public int Quality;

        public bool Dirty = true;

        protected bool ShouldApproximateNormals
        {
            get => Quality <= APPROXIMATE_NORMALS_QUALITY;
        }

        public static readonly float SIZE = 10;

        public ChunkSet OwningSet;

        public abstract GenTask CreateGetTask(ImplicitSurface sdf);

        public static Vector3[] ApproximateNormals(Vector3[] vertices, int[] triangles)
        {
            Vector3[] chunkNormals = new Vector3[vertices.Length];
            int[] vertexTrianglesCount = new int[vertices.Length];
            for (int iTriangle = 0; iTriangle < triangles.Length / 3; iTriangle++)
            {
                int iV0 = triangles[iTriangle * 3];
                int iV1 = triangles[iTriangle * 3 + 1];
                int iV2 = triangles[iTriangle * 3 + 2];

                Vector3 v0 = vertices[iV0];
                Vector3 v1 = vertices[iV1];
                Vector3 v2 = vertices[iV2];

                Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0);

                vertexTrianglesCount[iV0]++;
                vertexTrianglesCount[iV1]++;
                vertexTrianglesCount[iV2]++;

                chunkNormals[iV0] += normal;
                chunkNormals[iV1] += normal;
                chunkNormals[iV2] += normal;
            }
            for (int iVertex = 0; iVertex < vertices.Length; iVertex++)
            {
                chunkNormals[iVertex] = chunkNormals[iVertex].normalized;
            }

            return chunkNormals;
        }

        public static Vector3[] CalculateNormals(Vector3[] vertices, Vector3 samplingOffset, Vector3Expression norm)
        {
            Vector3[] chunkNormals = new Vector3[vertices.Length];
            for (int iChunkVertex = 0; iChunkVertex < vertices.Length; iChunkVertex++)
            {
                VariableInputSet<double> variableSet = ExtensionMethods.GetInputs(samplingOffset + vertices[iChunkVertex]);
                Vector3 n = norm.EvaluateOnce(variableSet).normalized;

                chunkNormals[iChunkVertex] = n;
            }
            return chunkNormals;
        }
    }
}