using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PointCloudMeshifier
{
    protected class Datas
    {
        public List<Vector3> chunkVertices = new List<Vector3>();
        public List<int> chunkTriangles = new List<int>();
        public ChunkVertexCache chunkVertexCache = new ChunkVertexCache(1);

        public void Clear(int nodes)
        {
            chunkVertices.Clear();
            chunkTriangles.Clear();

            if (chunkVertexCache == null || chunkVertexCache.Width != nodes)
            {
                chunkVertexCache = new ChunkVertexCache(nodes);
            }
            else
            {
                chunkVertexCache.Clear();
            }
        }
    }

    private readonly ObjectPool<Datas> pool;
    private readonly int padL;
    private readonly int padR;

    protected PointCloudMeshifier(int padL, int padR)
    {
        pool = new ObjectPool<Datas>(() => new Datas());

        this.padL = padL;
        this.padR = padR;
    }

    public void MarchIntoMesh(Mesh mesh, FeelerNodeSet nodes)
    {
        mesh.Clear();

        // Optimization because no triangles are needed if all nodes are the same
        if (nodes.IsUniform)
        {
            return;
        }

        // Get a set of spaces in memory to march into
        Datas space = pool.GetObject();
        space.Clear(nodes.Resolution);


        for (int x = padL; x < nodes.Resolution - padR; x++)
        {
            for (int y = padL; y < nodes.Resolution - padR; y++)
            {
                for (int z = padL; z < nodes.Resolution - padR; z++)
                {
                    GenerateForNode(space, nodes, new Vector3Int(x, y, z));
                }
            }
        }

        Vector3[] vertices = space.chunkVertices.ToArray();
        mesh.vertices = vertices;
        int[] triangles = space.chunkTriangles.ToArray();
        mesh.triangles = triangles;

        // Return datas
        pool.PutObject(space);
    }

    protected abstract void GenerateForNode(Datas space, FeelerNodeSet nodes, Vector3Int index);
}
