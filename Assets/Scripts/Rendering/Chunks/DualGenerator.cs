using System;
using UnityEngine;

namespace SDFRendering.Chunks
{
    public abstract class DualGenerator : PointCloudMeshifier
    {
        private enum Direction
        {
            X, Y, Z
        }

        protected DualGenerator()
            : base(0, 1, 1)
        {

        }

        protected abstract Vector3 CalculateVertex(PointCloud nodes, ImplicitSurface surface, Vector3Int index);

        protected override void GenerateForNode(MeshifierData space, PointCloud nodes, ImplicitSurface surface, Vector3Int index)
        {
            if (IsCellHomogenous(nodes, index))
            {
                return;
            }

            Vector3 vertex = CalculateVertex(nodes, surface, index);
            int[] vs = new int[4];
            vs[0] = space.GetOrAddVertex(index, 0, vertex);

            // Assume for default winding, surface goes from inside to outside as we go along the edge
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                if (!HasEdge(index, d))
                {
                    continue;
                }

                Vector3Int edgeOffset = GetEdgeOffset(d);
                if (nodes[index].SignBit == nodes[index + edgeOffset].SignBit)
                {
                    continue;
                }

                Vector3Int[] vertexOffsets = GetOffsets(d);
                for (int j = 0; j < 3; j++)
                {
                    Vector3Int offset = vertexOffsets[j];
                    vs[j + 1] = space.GetVertex(index + offset, 0);
                }

                bool isInside = nodes[index].Val < 0;
                if (isInside)
                {
                    space.AddToTriangles(vs[0], vs[1], vs[2]);
                    space.AddToTriangles(vs[0], vs[2], vs[3]);
                }
                else
                {
                    space.AddToTriangles(vs[0], vs[2], vs[1]);
                    space.AddToTriangles(vs[0], vs[3], vs[2]);
                }
            }
        }

        private static bool HasEdge(Vector3Int index, Direction d)
        {
            switch (d)
            {
                case Direction.X:
                    return index.y > 0 && index.z > 0;
                case Direction.Y:
                    return index.x > 0 && index.z > 0;
                case Direction.Z:
                    return index.x > 0 && index.y > 0;
                default:
                    throw new NotImplementedException();
            }
        }

        private static Vector3Int GetEdgeOffset(Direction d)
        {
            switch (d)
            {
                case Direction.X:
                    return new Vector3Int(1, 0, 0);
                case Direction.Y:
                    return new Vector3Int(0, 1, 0);
                case Direction.Z:
                    return new Vector3Int(0, 0, 1);
                default:
                    throw new NotImplementedException();
            }
        }

        private static Vector3Int[] GetOffsets(Direction d)
        {
            switch (d)
            {
                case Direction.X:
                    return new Vector3Int[]
                    {
                        new Vector3Int(0, -1, 0),
                        new Vector3Int(0, -1, -1),
                        new Vector3Int(0, 0, -1),
                    };
                case Direction.Y:
                    return new Vector3Int[]
                    {
                        new Vector3Int(0, 0, -1),
                        new Vector3Int(-1, 0, -1),
                        new Vector3Int(-1, 0, 0),
                    };
                case Direction.Z:
                    return new Vector3Int[]
                    {
                        new Vector3Int(-1, 0, 0),
                        new Vector3Int(-1, -1, 0),
                        new Vector3Int(0, -1, 0),
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        private bool IsCellHomogenous(PointCloud nodes, Vector3Int index)
        {
            Vector3Int[] cellOffsets = new Vector3Int[]
            {
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, 1, 1),
                new Vector3Int(1, 0, 0),
                new Vector3Int(1, 0, 1),
                new Vector3Int(1, 1, 0),
                new Vector3Int(1, 1, 1)
            };

            int sign = nodes[index].SignBit;

            foreach (var offset in cellOffsets)
            {
                if (nodes[index + offset].SignBit != sign)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
