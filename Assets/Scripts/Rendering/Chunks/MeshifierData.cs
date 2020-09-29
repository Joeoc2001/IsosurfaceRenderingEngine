using System;
using System.Collections.Generic;
using UnityEngine;

namespace SDFRendering.Chunks
{
    public class MeshifierData
    {
        private readonly List<Vector3> chunkVertices = new List<Vector3>();
        private readonly List<int> chunkTriangles = new List<int>();
        private ChunkVertexCache chunkVertexCache = new ChunkVertexCache(1, 1);

        public void Clear(int nodes, int depth)
        {
            chunkVertices.Clear();
            chunkTriangles.Clear();

            if (chunkVertexCache.Width != nodes || chunkVertexCache.Depth != depth)
            {
                chunkVertexCache = new ChunkVertexCache(nodes, depth);
            }
            else
            {
                chunkVertexCache.Clear();
            }
        }

        public int GetOrAddVertex(Vector3Int index, int depth, Vector3 vector)
        {
            return GetOrAddVertex(index, depth, () => vector);
        }

        public int GetOrAddVertex(Vector3Int index, int depth, Func<Vector3> vector)
        {

            int iChunkVertex;
            if (chunkVertexCache.IsSet(index, depth))
            {
                iChunkVertex = chunkVertexCache.Get(index, depth);
            }
            else
            {
                iChunkVertex = chunkVertices.Count;
                chunkVertices.Add(vector());

                chunkVertexCache.Set(index, depth, iChunkVertex);
            }
            return iChunkVertex;
        }

        public void AddToTriangles(int index1, int index2, int index3)
        {
            chunkTriangles.Add(index1);
            chunkTriangles.Add(index2);
            chunkTriangles.Add(index3);
        }

        public void PlaceInMesh(Mesh mesh)
        {
            mesh.Clear();

            Vector3[] vertices = chunkVertices.ToArray();
            mesh.vertices = vertices;
            int[] triangles = chunkTriangles.ToArray();
            mesh.triangles = triangles;
        }
    }
}