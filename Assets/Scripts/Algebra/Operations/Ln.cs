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

        public bool Equals(Ln other)
        {
            if (other is null)
            {
                return false;
            }

            return Argument.Equals(other.Argument);
        }

        public override bool Equals(Equation obj)
        {
            return this.Equals(obj as Ln);
        }

        public override int GenHashCode()
        {
            return Argument.GenHashCode() ^ -1043105826;
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