using UnityEngine;

namespace SDFRendering.Chunks
{
    public abstract class PointCloudMeshifier
    {

        private readonly ObjectPool<MeshifierData> _pool;
        private readonly int _padL;
        private readonly int _padR;
        private readonly int _depth;

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="padL">The number of elements of the point cloud to skip on the left for each dimension</param>
        /// <param name="padR">The number of elements of the point cloud to skip on the right for each dimension</param>
        /// <param name="depth">The possible number of vertices for each node</param>
        protected PointCloudMeshifier(int padL, int padR, int depth)
        {
            _pool = new ObjectPool<MeshifierData>(() => new MeshifierData());

            this._padL = padL;
            this._padR = padR;
            this._depth = depth;
        }

        public (Vector3[], int[]) MarchIntoMesh(PointCloud nodes, ImplicitSurface surface)
        {
            // Optimization because no triangles are needed if all nodes are the same
            if (nodes.IsUniform)
            {
                return (new Vector3[0], new int[0]);
            }

            // Get a set of spaces in memory to march into
            Vector3Int resolution = nodes.Resolution;
            MeshifierData space = _pool.GetObject();
            space.Clear(resolution, _depth);


            for (int x = _padL; x < resolution.x - _padR; x++)
            {
                for (int y = _padL; y < resolution.y - _padR; y++)
                {
                    for (int z = _padL; z < resolution.z - _padR; z++)
                    {
                        GenerateForNode(space, nodes, surface, new Vector3Int(x, y, z));
                    }
                }
            }

            (Vector3[] vertices, int[] triangles) = space.GetPolygons();

            // Return datas
            _pool.PutObject(space);

            return (vertices, triangles);
        }

        protected abstract void GenerateForNode(MeshifierData space, PointCloud nodes, ImplicitSurface surface, Vector3Int index);
    }
}