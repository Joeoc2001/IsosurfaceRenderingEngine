using System;
using UnityEngine;

namespace SDFRendering.Chunks.SurfaceNetChunk
{
    public class SurfaceNetGenerator : DualGenerator
    {
        public static readonly SurfaceNetGenerator Instance = new SurfaceNetGenerator();

        private SurfaceNetGenerator() : base(0, 1, 1)
        {

        }

        protected override Vector3 CalculateVertex(FeelerNodeSet nodes, Vector3Int index)
        {
            (Vector3Int, Vector3Int)[] edges = new (Vector3Int, Vector3Int)[]
            {
                ( new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1) ),
                ( new Vector3Int(0, 0, 0), new Vector3Int(0, 1, 0) ),
                ( new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0) ),
                ( new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 1) ),
                ( new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1) ),
                ( new Vector3Int(0, 1, 0), new Vector3Int(0, 1, 1) ),
                ( new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0) ),
                ( new Vector3Int(1, 0, 0), new Vector3Int(1, 0, 1) ),
                ( new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0) ),
                ( new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 1) ),
                ( new Vector3Int(1, 0, 1), new Vector3Int(1, 1, 1) ),
                ( new Vector3Int(1, 1, 0), new Vector3Int(1, 1, 1) ),
            };

            int count = 0;
            Vector3 total = Vector3.zero;
            foreach ((Vector3Int a, Vector3Int b) in edges)
            {
                FeelerNode nodeA = nodes[index + a];
                FeelerNode nodeB = nodes[index + b];

                if (nodeA.SignBit == nodeB.SignBit)
                {
                    continue;
                }

                float valA = nodeA.Val;
                float valB = nodeB.Val;
                float dist = valB / (valB - valA);
                Vector3 pos = dist * nodeA.Pos + (1 - dist) * nodeB.Pos;

                total += pos;
                count += 1;
            }

            return total / count;
        }
    }
}
