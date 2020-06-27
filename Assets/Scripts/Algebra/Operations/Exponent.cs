using Rationals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


namespace Algebra.Operations
{
    public class Exponent : Equation, IEquatable<Exponent>
    {
        public readonly Equation Base;
        public readonly Equation Power;

        new public static Equation Pow(Equation term, Equation power)
        {
            if (power.Equals(Constant.ZERO))
            {
                return 1;
            }

            if (power.Equals(Constant.ONE))
            {
                return term;
            }

            if (term is Constant termConstant && power is Constant exponentConstant)
            {
                Rational numerator = exponentConstant.GetValue().Numerator;
                Rational denominator = exponentConstant.GetValue().Denominator;
                if (numerator > -10 && numerator < 10) // Bounds for sanity sake
                {
                    Rational value = Rational.Pow(termConstant.GetValue(), (int)numerator);
                    if (value >= 0)
                    {
                        value = Rational.RationalRoot(value, (int)denominator);
                        return value;
                    }
                }
            }

            return new Exponent(term, power);
        }

        public Exponent(Equation term, Equation power)
        {
            this.Base = term;
            this.Power = power;
        }

        public override ExpressionDelegate GetExpression()
        {
            ExpressionDelegate termExp = Base.GetExpression();

            if (Power.Equals(-1))
            {
                return v => 1 / termExp(v);
            }

            ExpressionDelegate exponentExp = Power.GetExpression();

            return v => Mathf.Pow(termExp(v), exponentExp(v));
        }

        public override Equation GetDerivative(Variable wrt)
        {
            // Check for common cases
            if (Power is Constant powerConst)
            {
                Equation baseDerivative = Base.GetDerivative(wrt);
                return Power * baseDerivative * Pow(Base, powerConst.GetValue() - 1);
            }

            if (Base is Constant)
            {
                Equation exponentDerivative = Power.GetDerivative(wrt);
                return LnOf(Base) * exponentDerivative * this;
            }

            // Big derivative (u^v)'=(u^v)(vu'/u + v'ln(u))
            // Alternatively  (u^v)'=(u^(v-1))(vu' + uv'ln(u)) but I find the first form simplifies faster
            Equation baseDeriv = Base.GetDerivative(wrt);
            Equation expDeriv = Power.GetDerivative(wrt);
            return this * ((Power * baseDeriv / Base) + (expDeriv * LnOf(Base)));
        }

        public bool Equals(Exponent other)
        {
            if (other is null)
            {
                return false;
            }

            return Base.Equals(other.Base) && Power.Equals(other.Power);
        }

        public override bool Equals(Equation obj)
        {
            return this.Equals(obj as Exponent);
        }

        public override int GenHashCode()
        {
            return (31 * Base.GenHashCode() - Power.GenHashCode()) ^ 642859777;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(ToParenthesisedString(Base));
            builder.Append("^");
            builder.Append(ToParenthesisedString(Power));

            return builder.ToString();
        }

        public override string ToRunnableString()
        {
            return $"Equation.Pow({Base.ToRunnableString()}, {Power.ToRunnableString()})";
        }

        public override int GetOrderIndex()
        {
            return 10;
        }

        public override Equation Map(EquationMapping map)
        {
            Equation currentThis = this;

            if (map.ShouldMapChildren(this))
            {
                Equation mappedBase = Base.Map(map);
                Equation mappedPower = Power.Map(map);

                currentThis = Pow(mappedBase, mappedPower);
            }

            if (map.ShouldMapThis(this))
            {
                currentThis = map.PostMap(currentThis);
            }

            return currentThis;
        }
    }
}