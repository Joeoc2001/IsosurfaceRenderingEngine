using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * An immutable set of points, each with a position and sampled value 
 */
public class FeelerNodeSet : IEnumerable
{
    public readonly int Resolution;
    private readonly FeelerNode[,,] Nodes;
    private readonly bool areAllOutside = true;
    private readonly bool areAllInside = true;

    public FeelerNodeSet(EquationProvider body, int resolution, Vector3 offset, float delta)
    {
        Resolution = resolution;

        Equation distEquation = body.GetEquation().GetSimplifiedCached();
        Func<VectorN, float> distFunction = distEquation.GetExpressionCached();

        Nodes = new FeelerNode[resolution, resolution, resolution];
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int z = 0; z < resolution; z++)
                {
                    Vector3 pos = offset + new Vector3(delta * x, delta * y, delta * z);
                    float val = distFunction(new VectorN(pos));
                    Nodes[x, y, z] = new FeelerNode(pos, val);

                    if (val < 0)
                    {
                        areAllOutside = false;
                    } else
                    {
                        areAllInside = false;
                    }
                }
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
                    float v = Nodes[x, y, z].Val - nodes.Nodes[x, y, z].Val;
                    delta += v * v;
                }
            }
        }
        delta /= Resolution * Resolution * Resolution;
        return Mathf.Sqrt(delta);
    }

    public bool AreAllOutside()
    {
        return areAllOutside;
    }

    public bool AreAllInside()
    {
        return areAllInside;
    }

    public FeelerNode this[int x, int y, int z]
    {
        get
        {
            return Nodes[x, y, z];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Nodes.GetEnumerator();
    }
}
