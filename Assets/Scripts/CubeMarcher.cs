using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMarcher
{
    class MarchingDatas
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

    private static readonly ObjectPool<MarchingDatas> pool = new ObjectPool<MarchingDatas>(() => new MarchingDatas());

    public static void MarchIntoMesh(Mesh mesh, FeelerNodeSet nodes, Vector3 offset)
    {
        mesh.Clear();

        // Get a set of spaces in memory to march into
        MarchingDatas space = pool.GetObject();
        space.Clear(nodes.Resolution);

        // http://transvoxel.org/Lengyel-VoxelTerrain.pdf

        FeelerNode[] cell = new FeelerNode[8];
        for (int cellX = 1; cellX < nodes.Resolution; cellX++)
        {
            for (int cellY = 1; cellY < nodes.Resolution; cellY++)
            {
                for (int cellZ = 1; cellZ < nodes.Resolution; cellZ++)
                {
                    cell[0] = nodes[cellX - 1, cellY - 1, cellZ - 1];
                    cell[1] = nodes[cellX, cellY - 1, cellZ - 1];
                    cell[2] = nodes[cellX - 1, cellY, cellZ - 1];
                    cell[3] = nodes[cellX, cellY, cellZ - 1];
                    cell[4] = nodes[cellX - 1, cellY - 1, cellZ];
                    cell[5] = nodes[cellX, cellY - 1, cellZ];
                    cell[6] = nodes[cellX - 1, cellY, cellZ];
                    cell[7] = nodes[cellX, cellY, cellZ];

                    int casecode = cell[0].SignBit
                        | cell[1].SignBit << 1
                        | cell[2].SignBit << 2
                        | cell[3].SignBit << 3
                        | cell[4].SignBit << 4
                        | cell[5].SignBit << 5
                        | cell[6].SignBit << 6
                        | cell[7].SignBit << 7;

                    if (casecode == 0 || casecode == 255)
                    {
                        continue;
                    }

                    byte cellClass = Transvoxel.RegularCellClass[casecode];
                    Transvoxel.CellData cellData = Transvoxel.RegularCellData[cellClass];
                    ushort[] vertexDatas = Transvoxel.RegularVertexData[casecode];

                    for (int iCellTri = 0; iCellTri < cellData.VertexIndex.Length / 3; iCellTri++)
                    {
                        for (int iCellTriVertex = 0; iCellTriVertex < 3; iCellTriVertex++)
                        {
                            // TODO: Shared vertices
                            int iCellVertex = cellData.VertexIndex[iCellTri * 3 + iCellTriVertex];
                            ushort vData = vertexDatas[iCellVertex];

                            int iNode1 = vData & 0xF;
                            int iNode2 = (vData >> 4) & 0xF;

                            int axis = ((vData >> 8) & 0xF) - 1;

                            int offsetX = (vData >> 12) & 0x1;
                            int offsetY = (vData >> 13) & 0x1;
                            int offsetZ = (vData >> 14) & 0x1;

                            int vertexParentX = cellX - offsetX;
                            int vertexParentY = cellY - offsetY;
                            int vertexParentZ = cellZ - offsetZ;

                            int iChunkVertex;
                            if (space.chunkVertexCache.IsSet(vertexParentX, vertexParentY, vertexParentZ, axis))
                            {
                                iChunkVertex = space.chunkVertexCache.Get(vertexParentX, vertexParentY, vertexParentZ, axis);
                            }
                            else
                            {
                                FeelerNode node1 = cell[iNode1];
                                FeelerNode node2 = cell[iNode2];

                                Vector3 v;
                                float t = node1.Val / (node1.Val - node2.Val);
                                v = t * node2.Pos + (1 - t) * node1.Pos;
                                v -= offset;

                                iChunkVertex = space.chunkVertices.Count;
                                space.chunkVertices.Add(v);

                                space.chunkVertexCache.Set(vertexParentX, vertexParentY, vertexParentZ, axis, iChunkVertex);
                            }

                            space.chunkTriangles.Add(iChunkVertex);

                            int iChunkTriangle = (space.chunkTriangles.Count - 1) / 3;
                        }
                    }
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
}
