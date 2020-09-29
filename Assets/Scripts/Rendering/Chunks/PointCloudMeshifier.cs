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

        public void MarchIntoMesh(Mesh mesh, FeelerNodeSet nodes)
        {
            // Optimization because no triangles are needed if all nodes are the same
            if (nodes.IsUniform)
            {
                return;
            }

            // Get a set of spaces in memory to march into
            MeshifierData space = _pool.GetObject();
            space.Clear(nodes.Resolution, _depth);


            for (int x = _padL; x < nodes.Resolution - _padR; x++)
            {
                for (int y = _padL; y < nodes.Resolution - _padR; y++)
                {
                    for (int z = _padL; z < nodes.Resolution - _padR; z++)
                    {
                        GenerateForNode(space, nodes, new Vector3Int(x, y, z));
                    }
                }
            }

            space.PlaceInMesh(mesh);

            // Return datas
            _pool.PutObject(space);
        }

        protected abstract void GenerateForNode(MeshifierData space, FeelerNodeSet nodes, Vector3Int index);
    }
}