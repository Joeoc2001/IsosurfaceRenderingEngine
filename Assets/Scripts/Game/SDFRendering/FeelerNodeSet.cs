using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;

/**
 * An immutable set of points, each with a position and sampled value 
 */
public class FeelerNodeSet : IEnumerable
{
    public readonly int Resolution;
    private readonly FeelerNode[] Nodes;
    public readonly bool IsUniform;

    public FeelerNodeSet(int resolution, FeelerNode[] nodes)
    {
        Resolution = resolution;
        Nodes = nodes;

        IsUniform = true;
        bool allInside = true;
        bool allOutside = true;
        foreach (FeelerNode node in nodes)
        {
            if (node.Val > 0)
            {
                allInside = false;
            }
            else
            {
                allOutside = false;
            }

            if (!allInside && !allOutside)
            {
                IsUniform = false;
                break;
            }
        }
    }

    public float Delta(FeelerNodeSet nodes)
    {
        if (nodes.Resolution != Resolution)
        {
            return float.MaxValue;
        }

        float delta = 0;
        for (int x = 0; x < Resolution; x++)
        {
            for (int y = 0; y < Resolution; y++)
            {
                for (int z = 0; z < Resolution; z++)
                {
                    float v = this[x, y, z].Val - nodes[x, y, z].Val;
                    delta += v * v;
                }
            }
        }
        delta /= Resolution * Resolution * Resolution;
        return Mathf.Sqrt(delta);
    }

    public FeelerNode this[int x, int y, int z]
    {
        get
        {
            return Nodes[(x * Resolution + y) * Resolution + z];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Nodes.GetEnumerator();
    }
}
