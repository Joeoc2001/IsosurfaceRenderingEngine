using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Int3 : IEquatable<Int3>
{
    public int X { get { return _tuple.Item1; } }
    public int Y { get { return _tuple.Item2; } }
    public int Z { get { return _tuple.Item3; } }

    private readonly Tuple<int, int, int> _tuple;

    public Int3(int x, int y, int z)
    {
        _tuple = new Tuple<int, int, int>(x, y, z);
    }

    public Int3(Vector3 vector3)
        : this((int)vector3.x, (int)vector3.y, (int)vector3.z)
    { }

    public bool Equals(Int3 other)
    {
        if (other == null)
        {
            return false;
        }

        return _tuple.Equals(other._tuple);
    }

    public static bool operator ==(Int3 left, Int3 right)
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

    public static bool operator !=(Int3 left, Int3 right)
    {
        return !(left == right);
    }

    public static Int3 operator+(Int3 a, Int3 b)
    {
        return new Int3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Vector3 operator *(Int3 a, float s)
    {
        return new Vector3(a.X * s, a.Y * s, a.Z * s);
    }

    public static Vector3 operator *(float s, Int3 a)
    {
        return new Vector3(a.X * s, a.Y * s, a.Z * s);
    }

    public override int GetHashCode()
    {
        return _tuple.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Int3);
    }
}
