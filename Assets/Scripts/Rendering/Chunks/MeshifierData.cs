using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SDFRendering.Chunks
{
    public class MeshifierData
    {
        private readonly List<Vector3> _chunkVertices = new List<Vector3>();
        private readonly List<int> _chunkTriangles = new List<int>();
        private ChunkVertexCache _chunkVertexCache = new ChunkVertexCache(new Vector3Int(1, 1, 1), 1);

        public void Clear(Vector3Int nodes, int depth)
        {
            _chunkVertices.Clear();
            _chunkTriangles.Clear();

            if (_chunkVertexCache.Size != nodes || _chunkVertexCache.Depth != depth)
            {
                _chunkVertexCache = new ChunkVertexCache(nodes, depth);
            }
            else
            {
                _chunkVertexCache.Clear();
            }
        }

        public int GetOrAddVertex(Vector3Int index, int depth, Vector3 vector)
        {
            return GetOrAddVertex(index, depth, () => vector);
        }

        public int GetOrAddVertex(Vector3Int index, int depth, Func<Vector3> vector)
        {

            int iChunkVertex;
            if (_chunkVertexCache.IsSet(index, depth))
            {
                iChunkVertex = _chunkVertexCache.Get(index, depth);
            }
            else
            {
                iChunkVertex = _chunkVertices.Count;
                _chunkVertices.Add(vector());

                _chunkVertexCache.Set(index, depth, iChunkVertex);
            }
            return iChunkVertex;
        }

        public int GetVertex(Vector3Int index, int depth)
        {
            if (_chunkVertexCache.IsSet(index, depth))
            {
                return _chunkVertexCache.Get(index, depth);
            }
            else
            {
                throw new IndexOutOfRangeException("Vertex not set");
            }
        }

        public void AddToTriangles(int index1, int index2, int index3)
        {
            _chunkTriangles.Add(index1);
            _chunkTriangles.Add(index2);
            _chunkTriangles.Add(index3);
        }

        public void PlaceInMesh(Mesh mesh)
        {
            mesh.Clear();

            Vector3[] vertices = _chunkVertices.ToArray();
            mesh.vertices = vertices;
            int[] triangles = _chunkTriangles.ToArray();
            mesh.triangles = triangles;
        }
    }
}