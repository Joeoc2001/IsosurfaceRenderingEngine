using System;
using UnityEngine;

public unsafe struct VariableSet : IEquatable<VariableSet>
{
    private fixed float values[Variable.VariablesCount];

    public static implicit operator VariableSet(float vector) => new VariableSet(vector);
    public static implicit operator VariableSet(Vector2 vector) => new VariableSet(vector);
    public static implicit operator VariableSet(Vector3 vector) => new VariableSet(vector);
    public static implicit operator VariableSet(float[] vector) => new VariableSet(vector);

    public VariableSet(float vector)
    {
        values[(int)Variable.Variables.X] = vector;
    }

    public VariableSet(Vector2 vector)
    {
        values[(int)Variable.Variables.X] = vector.x;
        values[(int)Variable.Variables.Y] = vector.y;
    }

    public VariableSet(Vector3 vector)
    {
        values[(int)Variable.Variables.X] = vector.x;
        values[(int)Variable.Variables.Y] = vector.y;
        values[(int)Variable.Variables.Z] = vector.z;
    }

    public VariableSet(float[] vector)
    {
        int i = Math.Min(Variable.VariablesCount, vector.Length);
        for (int j = 0; j < i; j++)
        {
            values[j] = vector[j];
        }
    }

    public float this[Variable v]
    {
        get => values[v.Index];
    }

    public bool Equals(VariableSet o)
    {
        for (int i = 0; i < Variable.VariablesCount; i++)
        {
            if (!Mathf.Approximately(values[i], o.values[i]))
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
