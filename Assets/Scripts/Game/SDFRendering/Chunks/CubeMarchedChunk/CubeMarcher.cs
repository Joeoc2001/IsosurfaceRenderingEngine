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

    public static readonly CubeMarcher Instance = new CubeMarcher();

    private readonly ObjectPool<MarchingDatas> pool;

    private CubeMarcher()
    {
        pool = new ObjectPool<MarchingDatas>(() => new MarchingDatas());
    }

    public void MarchIntoMeshCore(Mesh mesh, FeelerNodeSet nodes, Vector3 offset)
    {
        mesh.Clear();

        // Optimization because no triangles are needed if all nodes are the same
        if (nodes.IsUniform)
        {
            return;
        }

        // Get a set of spaces in memory to march into
        MarchingDatas space = pool.GetObject();
        space.Clear(nodes.Resolution);

        // http://transvoxel.org/Lengyel-VoxelTerrain.pdf

        for (int cellX = 2; cellX < nodes.Resolution - 1; cellX++)
        {
            for (int cellY = 2; cellY < nodes.Resolution - 1; cellY++)
            {
                for (int cellZ = 2; cellZ < nodes.Resolution - 1; cellZ++)
                {
                    Vector3Int cellIndex = new Vector3Int(cellX, cellY, cellZ);
                    FeelerNode[] cell = ExtractCell(nodes, cellIndex);
                    AddRegularCellToData(space, cell, cellIndex, offset);
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

    private FeelerNode[] ExtractCell(FeelerNodeSet nodes, Vector3Int cellIndex)
    {
        FeelerNode[] cell = new FeelerNode[8];

        cell[0] = nodes[cellIndex.x - 1, cellIndex.y - 1, cellIndex.z - 1];
        cell[1] = nodes[cellIndex.x, cellIndex.y - 1, cellIndex.z - 1];
        cell[2] = nodes[cellIndex.x - 1, cellIndex.y, cellIndex.z - 1];
        cell[3] = nodes[cellIndex.x, cellIndex.y, cellIndex.z - 1];
        cell[4] = nodes[cellIndex.x - 1, cellIndex.y - 1, cellIndex.z];
        cell[5] = nodes[cellIndex.x, cellIndex.y - 1, cellIndex.z];
        cell[6] = nodes[cellIndex.x - 1, cellIndex.y, cellIndex.z];
        cell[7] = nodes[cellIndex.x, cellIndex.y, cellIndex.z];

        return cell;
    }

    private void AddRegularCellToData(MarchingDatas data, FeelerNode[] cell, Vector3Int cellIndex, Vector3 offset)
    {
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
            return;
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

                Vector3Int parent = cellIndex - new Vector3Int(offsetX, offsetY, offsetZ);

                int iChunkVertex;
                if (data.chunkVertexCache.IsSet(parent, axis))
                {
                    iChunkVertex = data.chunkVertexCache.Get(parent, axis);
                }
                else
                {
                    FeelerNode node1 = cell[iNode1];
                    FeelerNode node2 = cell[iNode2];

                    Vector3 v;
                    float t = node1.Val / (node1.Val - node2.Val);
                    v = t * node2.Pos + (1 - t) * node1.Pos;
                    v += offset;

                    iChunkVertex = data.chunkVertices.Count;
                    data.chunkVertices.Add(v);

                    data.chunkVertexCache.Set(parent, axis, iChunkVertex);
                }

                data.chunkTriangles.Add(iChunkVertex);
            }
        }
    }

    private void AddTransCellToData(MarchingDatas data, FeelerNode[] cell, Vector3Int cellIndex, Vector3 offset)
    {
    }
}
