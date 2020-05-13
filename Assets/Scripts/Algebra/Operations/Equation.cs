using Rationals;
using System;
using System.Collections.Generic;

public abstract class Equation
{
    public static implicit operator Equation(int r) => (Constant)r;
    public static implicit operator Equation(long r) => (Constant)r;
    public static implicit operator Equation(float r) => (Constant)r;
    public static implicit operator Equation(double r) => (Constant)r;
    public static implicit operator Equation(decimal r) => (Constant)r;
    public static implicit operator Equation(Rational r) => (Constant)r;

    public delegate float ExpressionDelegate(VariableSet set);

    public abstract ExpressionDelegate GetExpression();

    public abstract Equation GetDerivative(Variable wrt);

    [Obsolete("GetSimplified is deprecated, it is no longer necissary and should do nothing.")]
    public Equation GetSimplified() { return this; }

    public static Equation operator +(Equation left, Equation right)
    {
        return Add(new List<Equation>() { left, right });
    }

    public static Equation operator -(Equation left, Equation right)
    {
        return Add(new List<Equation>() { left, -1 * right });
    }

    public static Equation Add(List<Equation> eqs)
    {
        return Addition.Add(eqs);
    }

    public static Equation operator *(Equation left, Equation right)
    {
        return Multiply(new List<Equation>() { left, right });
    }

    public static Equation operator /(Equation left, Equation right)
    {
        // a/b = a * (b^-1)
        return Multiply(new List<Equation>() { left, Pow(right, -1) });
    }

    public static Equation Multiply(List<Equation> eqs)
    {
        return Multiplication.Multiply(eqs);
    }

    public static Equation Pow(Equation left, Equation right)
    {
        return Exponentiation.Pow(left, right);
    }

    public static Equation Ln(Equation left)
    {
        return LnOperation.Ln(left);
    }

    public override abstract int GetHashCode();

    public override abstract bool Equals(object obj);

    public abstract string ToParsableString();
    public abstract string ToRunnableString();

    public static bool operator ==(Equation left, Equation right)
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

    public static bool operator !=(Equation left, Equation right)
    {
        return !(left == right);
    }
}
