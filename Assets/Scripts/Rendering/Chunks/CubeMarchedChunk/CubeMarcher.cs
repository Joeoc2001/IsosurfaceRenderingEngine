using UnityEngine;

namespace SDFRendering.Chunks.CubeMarchedChunk
{
    public class CubeMarcher : PointCloudMeshifier
    {
        public static readonly CubeMarcher Instance = new CubeMarcher();

        private CubeMarcher()
            : base(1, 0, 3)
        {
        }

        protected override void GenerateForNode(MeshifierData space, FeelerNodeSet nodes, Vector3Int index)
        {
            FeelerNode[] cell = ExtractCell(nodes, index);
            AddRegularCellToData(space, cell, index);
        }

        private FeelerNode[] ExtractCell(FeelerNodeSet nodes, Vector3Int cellIndex)
        {
            FeelerNode[] cell = new FeelerNode[8];

            cell[0] = nodes[cellIndex + new Vector3Int(-1, -1, -1)];
            cell[1] = nodes[cellIndex + new Vector3Int(0, -1, -1)];
            cell[2] = nodes[cellIndex + new Vector3Int(-1, 0, -1)];
            cell[3] = nodes[cellIndex + new Vector3Int(0, 0, -1)];
            cell[4] = nodes[cellIndex + new Vector3Int(-1, -1, 0)];
            cell[5] = nodes[cellIndex + new Vector3Int(0, -1, 0)];
            cell[6] = nodes[cellIndex + new Vector3Int(-1, 0, 0)];
            cell[7] = nodes[cellIndex + new Vector3Int(0, 0, 0)];

            return cell;
        }

        private void AddRegularCellToData(MeshifierData data, FeelerNode[] cell, Vector3Int cellIndex)
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
                int[] triangle = new int[3];
                for (int iCellTriVertex = 0; iCellTriVertex < 3; iCellTriVertex++)
                {
                    int iCellVertex = cellData.VertexIndex[iCellTri * 3 + iCellTriVertex];
                    ushort vData = vertexDatas[iCellVertex];

                    int iNode1 = vData & 0xF;
                    int iNode2 = (vData >> 4) & 0xF;

                    int axis = ((vData >> 8) & 0xF) - 1;

                    int offsetX = (vData >> 12) & 0x1;
                    int offsetY = (vData >> 13) & 0x1;
                    int offsetZ = (vData >> 14) & 0x1;

                    Vector3Int parent = cellIndex - new Vector3Int(offsetX, offsetY, offsetZ);

                    Vector3 generateVertex()
                    {
                        FeelerNode node1 = cell[iNode1];
                        FeelerNode node2 = cell[iNode2];

                        float t = node1.Val / (node1.Val - node2.Val);
                        return t * node2.Pos + (1 - t) * node1.Pos;
                    }

                    int iChunkVertex = data.GetOrAddVertex(parent, axis, generateVertex);
                    triangle[iCellTriVertex] = iChunkVertex;
                }

                data.AddToTriangles(triangle[0], triangle[1], triangle[2]);
            }
        }
    }
}