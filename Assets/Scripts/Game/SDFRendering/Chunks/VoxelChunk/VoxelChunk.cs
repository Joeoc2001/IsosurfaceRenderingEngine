using Algebra;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

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
    public void GenerateMesh(FeelerNodeSet nodes, Equation.Vector3ExpressionDelegate norm)
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

    public override PriorGenTask CreatePriorJob(SDF sdf)
    {
        return new VoxelPriorGenTask(this, sdf);
    }
}
