using Rationals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Sign : Monad
{
    new public static Equation SignOf(Equation argument)
    {
        if (argument is Sign)
        {
            return argument;
        }

        if (argument is Constant constant)
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

        return new Sign(argument);
    }

    private Sign(Equation argument)
        : base(argument)
    {

    }

    public override Equation GetDerivative(Variable wrt)
    {
        return 0; // Not always true, but true 100% of the time :P
    }

    public override ExpressionDelegate GetExpression()
    {
        ExpressionDelegate eqExpression = Argument.GetExpression();
        return v => Math.Sign(eqExpression(v));
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Monad);
    }

    public override int GetHashCode()
    {
        return Argument.GetHashCode() ^ -322660314;
    }

    public override string ToString()
    {
        return $"[SIGN]({Argument})";
    }

    public override string ToParsableString()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("sign ");
        builder.Append(ParenthesisedParsableString(Argument));

        return builder.ToString();
    }

    public override string ToRunnableString()
    {
        return $"Equation.SignOf({Argument.ToRunnableString()})";
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

    public override int GetOrderIndex()
    {
        return 0;
    }
}
