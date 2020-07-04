using Algebra;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CubeMarchedChunk : Chunk
{
    public static readonly int APPROXIMATE_NORMALS_QUALITY = 2;

    private Mesh coreMesh;
    private Mesh transitionMesh;

    private Mesh totalMesh;

    [Range(1, 10)]
    public int quality;

    private bool ShouldApproximateNormals
    {
        get => quality <= APPROXIMATE_NORMALS_QUALITY;
    }

    void Awake()
    {
        coreMesh = new Mesh();
        transitionMesh = new Mesh();

        totalMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = totalMesh;
    }

    // Returns true if the mesh changed
    public void GenerateMeshCore(FeelerNodeSet nodes, Equation.Vector3ExpressionDelegate norm)
    {
        CubeMarcher.Instance.MarchIntoMeshCore(coreMesh, nodes, -transform.position);

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
            // Calculate normals
            Vector3[] chunkNormals = new Vector3[vertices.Length];
            for (int iChunkVertex = 0; iChunkVertex < vertices.Length; iChunkVertex++)
            {
                VariableSet VariableSet = new VariableSet(transform.position + vertices[iChunkVertex]);
                Vector3 n = norm(VariableSet).normalized;

                chunkNormals[iChunkVertex] = n;
            }
            coreMesh.normals = chunkNormals;
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
            chunkNormals[iVertex] = chunkNormals[iVertex].normalized;
        }

        return chunkNormals;
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

    public override PriorGenTask CreatePriorJob(SDF sdf)
    {
        return new CubeMarchPriorGenTask(this, sdf);
    }

    public override void SetQuality(int quality)
    {
        this.quality = quality;
    }
}
