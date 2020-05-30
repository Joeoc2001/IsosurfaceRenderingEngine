using Rationals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LnOperation : Equation, IEquatable<LnOperation>
{
    private readonly Equation eq;

    new public static Equation Ln(Equation eq)
    {
        return new LnOperation(eq);
    }

    public LnOperation(Equation eq)
    {
        this.eq = eq;
    }

    public override ExpressionDelegate GetExpression()
    {
        if (eq is Constant constant)
        {
            return v => (float)Rational.Log(constant.GetValue());
        }

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
        return eq.GetHashCode() ^ -1043105826;
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
        return $"[LN]({eq})";
    }

    public override string ToParsableString()
    {
        return $"ln {eq.ToParsableString()}";
    }

    public override string ToRunnableString()
    {
        return $"Equation.Ln({eq.ToRunnableString()})";
    }
}
