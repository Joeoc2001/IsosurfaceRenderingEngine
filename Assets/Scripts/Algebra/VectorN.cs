using System;
using System.Linq;
using UnityEngine;

public class VectorN : IEquatable<VectorN>
{
    private readonly float[] vs;

    public VectorN(float[] source)
    {
        vs = new float[source.Length];
        source.CopyTo(vs, 0);
    }

    public VectorN(float source)
    {
        vs = new float[] { source };
    }

    public VectorN(Vector2 source)
    {
        vs = new float[]{ source.x, source.y };
    }

    public VectorN(Vector3 source)
    {
        vs = new float[] { source.x, source.y, source.z };
    }

    public VectorN(Vector4 source)
    {
        vs = new float[] { source.x, source.y, source.z, source.w };
    }

    public float this[int i]
    {
        get => vs[i];
    }

    public bool Equals(VectorN obj)
    {
        if (obj == null)
        {
            return false;
        }

        return vs.SequenceEqual(obj.vs);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as VectorN);
    }

    public override int GetHashCode()
    {
        return vs.GetHashCode();
    }

    public override string ToString()
    {
        return String.Format("Vector <{0}>", vs.ToString());
    }

    public static bool operator ==(VectorN left, VectorN right)
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

    public static bool operator !=(VectorN left, VectorN right)
    {
        return !(left == right);
    }
}
