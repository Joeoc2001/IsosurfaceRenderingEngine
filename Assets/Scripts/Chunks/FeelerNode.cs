using System;
using UnityEngine;

public struct FeelerNode
{
    public readonly Vector3 Pos;
    public readonly float Val;

    // Precached values
    public readonly int SignBit;

    public FeelerNode(Vector3 pos, float val)
    {
        Pos = pos;
        Val = val;

        SignBit = (-Math.Sign(Val) + 1) / 2;
    }
}
