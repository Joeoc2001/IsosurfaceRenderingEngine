using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Chunk : MonoBehaviour
{
    Mesh mesh;

    [Range(0.1f, 100f)]
    public float size;

    public EquationProvider distanceField;

    [Range(1, 10)]
    public int quality;

    public bool approximateNormals;
    public bool showGizmoFeelers;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    FeelerNodeSet GetFeelerNodes()
    {
        // Per side
        int numNodes = (1 << quality) + 1;
        float delta = size / (1 << quality);

        return new FeelerNodeSet(distanceField, numNodes, transform.position, delta);
    }

    public void GenerateMesh()
    {
        if (mesh is null || distanceField is null)
        {
            return;
        }

        FeelerNodeSet nodes = GetFeelerNodes();

        CubeMarcher.MarchIntoMesh(mesh, nodes, transform.position);

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
            // Calculate normal function
            Equation distEquation = distanceField.GetEquation();
            Func<VectorN, float> dx = distEquation.GetDerivative(Variable.X).GetExpression();
            Func<VectorN, float> dy = distEquation.GetDerivative(Variable.Y).GetExpression();
            Func<VectorN, float> dz = distEquation.GetDerivative(Variable.Z).GetExpression();

            // Calculate normals
            Vector3[] chunkNormals = new Vector3[vertices.Length];
            for (int iChunkVertex = 0; iChunkVertex < vertices.Length; iChunkVertex++)
            {
                VectorN pos = new VectorN(transform.position + vertices[iChunkVertex]);
                Vector3 n = new Vector3(dx(pos), dy(pos), dz(pos)).normalized;

                chunkNormals[iChunkVertex] = n;
            }
            mesh.normals = chunkNormals;
        }
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

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        if (showGizmoFeelers)
        {
            foreach (FeelerNode n in GetFeelerNodes())
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
