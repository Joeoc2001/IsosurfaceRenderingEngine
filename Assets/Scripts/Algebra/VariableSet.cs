using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public unsafe struct VariableSet : IEquatable<VariableSet>
{
    private fixed float values[Variable.VariablesCount];

    public VariableSet(float3 vector)
    {
        values[0] = vector.x;
        values[1] = vector.y;
        values[2] = vector.z;
    }

    public float this[Variable v]
    {
        get => values[v.Index];
        set => values[v.Index] = value;
    }

    public bool Equals(VariableSet o)
    {
        for (int i = 0; i < Variable.VariablesCount; i++)
        {
            if (!Mathf.Approximately(values[i],o.values[i]))
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
