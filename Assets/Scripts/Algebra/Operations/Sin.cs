using Rationals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;


namespace Algebra.Operations
{
    public class Sin : Monad
    {
        new public static Equation SinOf(Equation argument)
        {
            return new Sin(argument);
        }

        public Sin(Equation argument)
            : base(argument)
        {

        }

        public override ExpressionDelegate GetExpression()
        {
            ExpressionDelegate expression = Argument.GetExpression();

            return v => Mathf.Sin(expression(v));
        }

        public override Equation GetDerivative(Variable wrt)
        {
            Equation derivative = Argument.GetDerivative(wrt);
            return derivative * CosOf(Argument);
        }

        public bool Equals(Sin other)
        {
            if (other is null)
            {
                return false;
            }

            return Argument.Equals(other.Argument);
        }

        public override bool Equals(Equation obj)
        {
            return this.Equals(obj as Sin);
        }

        public override int GenHashCode()
        {
            return Argument.GenHashCode() ^ -1010034057;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("sin ");
            builder.Append(ToParenthesisedString(Argument));

            return builder.ToString();
        }

        public override string ToRunnableString()
        {
            return $"Equation.SinOf({Argument.ToRunnableString()})";
        }

        public override int GetOrderIndex()
        {
            return 0;
        }

        public override Func<Equation, Equation> GetSimplifyingConstructor()
        {
            return SinOf;
        }
    }
}