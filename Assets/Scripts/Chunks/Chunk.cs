using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
public class Chunk : MonoBehaviour
{
    Mesh mesh;

    [Range(0f, 5f)]
    public float updateThreshold;

    [Range(0.1f, 100f)]
    public float size;

    [Range(1, 10)]
    public int quality;

    public bool approximateNormals;
    public bool showGizmoFeelers;

    public bool IsEmpty { get; private set; } = false;

    private FeelerNodeSet lastNodes = null;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Returns true if the mesh changed
    public bool GenerateMesh(FeelerNodeSet nodes, Func<VariableSet, Vector3> norm)
    {
        if (mesh is null)
        {
            return false;
        }

        // Optimization because no triangles are needed if all nodes are the same
        IsEmpty = false;
        if (nodes.IsUniform)
        {
            IsEmpty = true;
            mesh.Clear();
            return true;
        }

        CubeMarcher.Instance.MarchIntoMesh(mesh, nodes, transform.position);

        // Extract for speed
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Calculate normals
        if (approximateNormals)
        {
            mesh.normals = ApproximateNormals(vertices, triangles);
        }
        else
        {
            // Calculate normals
            Vector3[] chunkNormals = new Vector3[vertices.Length];
            for (int iChunkVertex = 0; iChunkVertex < vertices.Length; iChunkVertex++)
            {
                VariableSet variableSet = new VariableSet(transform.position + vertices[iChunkVertex]);
                Vector3 n = norm(variableSet).normalized;

                chunkNormals[iChunkVertex] = n;
            }
            mesh.normals = chunkNormals;
        }
        return true;
    }

    private static Vector3[] ApproximateNormals(Vector3[] vertices, int[] triangles)
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
            chunkNormals[iVertex] /= vertexTrianglesCount[iVertex];
        }

        return chunkNormals;
    }

    private void OnDrawGizmos()
    {
        if (showGizmoFeelers)
        {
            if (!(lastNodes is null))
            {
                foreach (FeelerNode n in lastNodes)
                {
                    if (n.Val < 0)
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawSphere(n.Pos, .05f);
                }
            }
        }
    }
}
