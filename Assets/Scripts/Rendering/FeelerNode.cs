using System;
using Unity.Mathematics;
using UnityEngine;

namespace SDFRendering
{
    public struct FeelerNode
    {
        public readonly float3 Pos;
        public readonly float Val;

        public readonly int SignBit;

        public FeelerNode(float3 pos, float val)
        {
            Pos = pos;
            Val = val;


            if (float.IsNaN(Val))
            {
                SignBit = 1;
            }
            else
            {
                SignBit = (-Math.Sign(Val) + 1) / 2;
            }
        }
    }
}