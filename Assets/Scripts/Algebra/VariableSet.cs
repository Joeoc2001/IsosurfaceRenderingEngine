using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public struct VariableSet : IEquatable<VariableSet>
{
    private float[] values;

    public VariableSet(float3 vector)
    {
        values = null;
        Set(vector);
    }

    public void Set(float3 vector)
    {
        if (values is null)
        {
            values = new float[Variable.VariableDict.Count];
        }

        values[(int)Variable.Variables.X] = vector.x;
        values[(int)Variable.Variables.Y] = vector.y;
        values[(int)Variable.Variables.Z] = vector.z;
    }

    public float this[Variable v]
    {
        get => values[v.Index];
        set => values[v.Index] = value;
    }

    public bool Equals(VariableSet other)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (!Mathf.Approximately(values[i], other.values[i]))
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        if (obj is null || !(obj is VariableSet))
        {
            return false;
        }

        return this.Equals((VariableSet)obj);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException("Variable sets cannot be hashed"); // No hash function can exist for this due to floating point accuracy
    }

    public static bool operator ==(VariableSet left, VariableSet right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        return left.Equals(right);
    }

    public static bool operator !=(VariableSet left, VariableSet right)
    {
        return !(left == right);
    }
}
