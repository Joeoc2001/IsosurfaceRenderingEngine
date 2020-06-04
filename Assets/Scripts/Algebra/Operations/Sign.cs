using Rationals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Sign : Equation, IEquatable<Sign>
{
    private readonly Equation eq;

    new public static Equation SignOf(Equation eq)
    {
        if (eq is Sign)
        {
            return eq;
        }

        if (eq is Constant constant)
        {
            if (constant.GetValue().IsZero)
            {
                return 0;
            }
            if (constant.GetValue() > 0)
            {
                return 1;
            }
            if (constant.GetValue() < 0)
            {
                return -1;
            }
        }

        return new Sign(eq);
    }

    private Sign(Equation eq)
    {
        this.eq = eq;
    }

    public override Equation GetDerivative(Variable wrt)
    {
        return 0; // Not technically true, but true 100% of the time :P
    }

    public override ExpressionDelegate GetExpression()
    {
        ExpressionDelegate eqExpression = eq.GetExpression();
        return v => Math.Sign(eqExpression(v));
    }

    public bool Equals(Sign obj)
    {
        if (obj is null)
        {
            return false;
        }

        return eq.Equals(obj.eq);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Sign);
    }

    public override int GetHashCode()
    {
        return eq.GetHashCode() ^ -322660314;
    }

    public override string ToString()
    {
        return $"[SIGN]({eq})";
    }

    public override string ToParsableString()
    {
        return $"sign {eq.ToParsableString()}";
    }

    public override string ToRunnableString()
    {
        return $"Equation.Sign({eq.ToRunnableString()})";
    }

    public static bool operator==(Sign left, Sign right)
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

    public static bool operator !=(Sign left, Sign right)
    {
        return !(left == right);
    }
}
