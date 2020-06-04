using Rationals;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equation
{
    public static implicit operator Equation(int r) => (Constant)r;
    public static implicit operator Equation(long r) => (Constant)r;
    public static implicit operator Equation(float r) => (Constant)r;
    public static implicit operator Equation(double r) => (Constant)r;
    public static implicit operator Equation(decimal r) => (Constant)r;
    public static implicit operator Equation(Rational r) => (Constant)r;

    public delegate float ExpressionDelegate(IVariableSet set);
    public delegate Vector2 Vector2ExpressionDelegate(IVariableSet set);
    public delegate Vector3 Vector3ExpressionDelegate(IVariableSet set);
    public delegate Vector4 Vector4ExpressionDelegate(IVariableSet set);

    public abstract ExpressionDelegate GetExpression();

    public abstract Equation GetDerivative(Variable wrt);

    public ExpressionDelegate GetDerivitiveExpression(Variable wrt)
    {
        return GetDerivative(wrt).GetExpression();
    }

    public Vector2ExpressionDelegate GetDerivitiveExpressionWrtXY()
    {
        ExpressionDelegate dxFunc = GetDerivative(Variable.X).GetExpression();
        ExpressionDelegate dyFunc = GetDerivative(Variable.Y).GetExpression();
        return (IVariableSet v) => new Vector2(dxFunc(v), dyFunc(v));
    }

    public Vector3ExpressionDelegate GetDerivitiveExpressionWrtXYZ()
    {
        ExpressionDelegate dxFunc = GetDerivative(Variable.X).GetExpression();
        ExpressionDelegate dyFunc = GetDerivative(Variable.Y).GetExpression();
        ExpressionDelegate dzFunc = GetDerivative(Variable.Z).GetExpression();
        return (IVariableSet v) => new Vector3(dxFunc(v), dyFunc(v), dzFunc(v));
    }

    public Vector4ExpressionDelegate GetDerivitiveExpressionWrtXYZW()
    {
        ExpressionDelegate dxFunc = GetDerivative(Variable.X).GetExpression();
        ExpressionDelegate dyFunc = GetDerivative(Variable.Y).GetExpression();
        ExpressionDelegate dzFunc = GetDerivative(Variable.Z).GetExpression();
        ExpressionDelegate dwFunc = GetDerivative(Variable.W).GetExpression();
        return (IVariableSet v) => new Vector4(dxFunc(v), dyFunc(v), dzFunc(v), dwFunc(v));
    }

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
        return Sum.Add(eqs);
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
        return Product.Multiply(eqs);
    }

    public static Equation Pow(Equation left, Equation right)
    {
        return Exponent.Pow(left, right);
    }

    public static Equation LnOf(Equation eq)
    {
        return Ln.LnOf(eq);
    }

    public static Equation SignOf(Equation eq)
    {
        return Sign.SignOf(eq);
    }

    public static Equation Abs(Equation eq)
    {
        return eq * SignOf(eq);
    }

    public static Equation Min(Equation a, Equation b)
    {
        return 0.5 * (a + b - Abs(a - b));
    }

    public static Equation Max(Equation a, Equation b)
    {
        return 0.5 * (a + b + Abs(a - b));
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
