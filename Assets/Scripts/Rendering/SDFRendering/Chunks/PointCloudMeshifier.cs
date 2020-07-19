using UnityEngine;

public abstract class PointCloudMeshifier
{

    private readonly ObjectPool<MeshifierData> pool;
    private readonly int padL;
    private readonly int padR;
    private readonly int depth;

    /// <summary>
    /// Protected constructor
    /// </summary>
    /// <param name="padL">The number of elements of the point cloud to skip on the left for each dimension</param>
    /// <param name="padR">The number of elements of the point cloud to skip on the right for each dimension</param>
    /// <param name="depth">The possible number of vertices for each node</param>
    protected PointCloudMeshifier(int padL, int padR, int depth)
    {
        pool = new ObjectPool<MeshifierData>(() => new MeshifierData());

        this.padL = padL;
        this.padR = padR;
        this.depth = depth;
    }

    public void MarchIntoMesh(Mesh mesh, FeelerNodeSet nodes)
    {
        // Optimization because no triangles are needed if all nodes are the same
        if (nodes.IsUniform)
        {
            return;
        }

        // Get a set of spaces in memory to march into
        MeshifierData space = pool.GetObject();
        space.Clear(nodes.Resolution, depth);


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

        space.PlaceInMesh(mesh);

        // Return datas
        pool.PutObject(space);
    }

    protected abstract void GenerateForNode(MeshifierData space, FeelerNodeSet nodes, Vector3Int index);
}
