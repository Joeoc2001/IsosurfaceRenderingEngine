using Rationals;
using System;
using UnityEngine;

public class Constant : Equation, IEquatable<Constant>
{
    public static readonly decimal TOLERANCE = 0.00000000001M;

    public static readonly Constant ZERO = new Constant(0);
    public static readonly Constant ONE = new Constant(1);
    public static readonly Constant MINUS_ONE = new Constant(-1);

    private readonly Rational value;

    public Constant(Rational value)
    {
        this.value = value;
    }

    public override ExpressionDelegate GetExpression()
    {
        float approximation = (float)value;
        return v => approximation;
    }

    public override Equation GetDerivative(Variable wrt)
    {
        return Constant.ZERO;
    }

    public bool Equals(Constant obj)
    {
        if (obj == null)
        {
            return false;
        }

        return value.Equals(obj.value);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Constant);
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override string ToString()
    {
        if (value.Denominator < 1000)
        {
            return $"{value}";
        }
        return $"[{(float)value:0.00}...]";
    }

    public override Equation GetSimplified()
    {
        if (!value.IsCanonical)
        {
            return new Constant(value.CanonicalForm);
        }
        return this;
    }

    public static bool operator ==(Constant left, Constant right)
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

    public static bool operator !=(Constant left, Constant right)
    {
        return !(left == right);
    }

    public Rational GetValue()
    {
        return value;
    }
}
