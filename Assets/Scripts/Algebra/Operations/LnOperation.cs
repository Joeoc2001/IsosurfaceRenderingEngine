using Rationals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LnOperation : Equation, IEquatable<LnOperation>
{
    private readonly Equation eq;

    public LnOperation(Equation eq)
    {
        this.eq = eq;
    }

    public override ExpressionDelegate GetExpression()
    {
        ExpressionDelegate expression = eq.GetExpression();

        return v => Mathf.Log(expression(v));
    }

    public override Equation GetDerivative(Variable wrt)
    {
        Equation derivative = eq.GetDerivative(wrt);
        return derivative / eq;
    }

    public bool Equals(LnOperation other)
    {
        if (other is null)
        {
            return false;
        }

        return eq.Equals(other.eq);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as LnOperation);
    }

    public override int GetHashCode()
    {
        return eq.GetHashCode();
    }

    public static bool operator ==(LnOperation left, LnOperation right)
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

    public static bool operator !=(LnOperation left, LnOperation right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"ln({eq})";
    }

    public override Equation GetSimplified()
    {
        Equation newEq = eq.GetSimplified();

        if (newEq is Constant newEqConstant)
        {
            return new Constant((Rational)Rational.Log(newEqConstant.GetValue()));
        }

        if (newEq.Equals(eq))
        {
            return this;
        }

        return new LnOperation(newEq).GetSimplified();
    }
}
