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
                }
            }
        }
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
