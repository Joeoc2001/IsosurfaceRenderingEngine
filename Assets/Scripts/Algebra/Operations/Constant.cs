using Rationals;
using System;
using UnityEngine;


namespace Algebra.Operations
{
    public class Constant : Equation, IEquatable<Constant>
    {
        public static readonly Constant ZERO = 0;
        public static readonly Constant ONE = 1;
        public static readonly Constant MINUS_ONE = -1;

        public static implicit operator Constant(int r) => Constant.From(r);
        public static implicit operator Constant(long r) => Constant.From(r);
        public static implicit operator Constant(float r) => Rational.Approximate(r);
        public static implicit operator Constant(double r) => Rational.Approximate(r);
        public static implicit operator Constant(decimal r) => Rational.Approximate(r);
        public static implicit operator Constant(Rational r) => Constant.From(r);

        private readonly Rational value;

        public static Constant From(Rational value)
        {
            return new Constant(value.CanonicalForm);
        }

        private Constant(Rational value)
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
            return 0;
        }

        public bool Equals(Constant obj)
        {
            if (obj == null)
            {
                return false;
            }

            return value.Equals(obj.value);
        }

        public override bool Equals(Equation obj)
        {
            return this.Equals(obj as Constant);
        }

        public override int GenHashCode()
        {
            return value.GetHashCode();
        }

        public Rational GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return $"{value}";
        }

        public override string ToRunnableString()
        {
            return $"Constant.From((Rational)({value.Numerator})/({value.Denominator}))";
        }

        public override int GetOrderIndex()
        {
            return 0;
        }

        public override Equation Map(EquationMapping map)
        {
            if (!map.ShouldMapThis(this))
            {
                return this;
            }

            return map.PostMap(this);
        }
    }
}