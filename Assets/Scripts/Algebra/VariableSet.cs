using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VariableSet : IEquatable<VariableSet>
{
    private readonly float[] values;

    public VariableSet()
    {
        values = new float[Variable.VariableDict.Count];
    }

    public void Set(Vector3 vector)
    {
        this[Variable.X] = vector.x;
        this[Variable.Y] = vector.y;
        this[Variable.Z] = vector.z;
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
        return this.Equals(obj as VariableSet);
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

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(VariableSet left, VariableSet right)
    {
        return !(left == right);
    }
}
