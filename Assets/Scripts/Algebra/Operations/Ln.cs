using Rationals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;


namespace Algebra.Operations
{
    public class Ln : Monad
    {
        new public static Equation LnOf(Equation argument)
        {
            return new Ln(argument);
        }

        public Ln(Equation argument)
            : base(argument)
        {

        }

        public override ExpressionDelegate GetExpression()
        {
            if (Argument is Constant constant)
            {
                return v => (float)Rational.Log(constant.GetValue());
            }

            ExpressionDelegate expression = Argument.GetExpression();

            return v => Mathf.Log(expression(v));
        }

        public override Equation GetDerivative(Variable wrt)
        {
            Equation derivative = Argument.GetDerivative(wrt);
            return derivative / Argument;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Ln);
        }

        public override int GetHashCode()
        {
            return Argument.GetHashCode() ^ -1043105826;
        }

        public static bool operator ==(Ln left, Ln right)
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

        public static bool operator !=(Ln left, Ln right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("ln ");
            builder.Append(ToParenthesisedString(Argument));

            return builder.ToString();
        }

        public override string ToRunnableString()
        {
            return $"Equation.Ln({Argument.ToRunnableString()})";
        }

        public override int GetOrderIndex()
        {
            return 0;
        }

        public override Func<Equation, Equation> GetSimplifyingConstructor()
        {
            return LnOf;
        }
    }
}