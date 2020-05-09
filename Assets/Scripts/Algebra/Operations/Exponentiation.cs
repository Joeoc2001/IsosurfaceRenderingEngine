using Rationals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exponentiation : Equation, IEquatable<Exponentiation>
{
    public readonly Equation Base;
    public readonly Equation Exponent;

    public Exponentiation(Equation term, Equation exponent)
    {
        this.Base = term;
        this.Exponent = exponent;
    }

    public override ExpressionDelegate GetExpression()
    {
        ExpressionDelegate termExp = Base.GetExpression();

        if (Exponent.Equals(Constant.MINUS_ONE))
        {
            return v => 1 / termExp(v);
        }

        ExpressionDelegate exponentExp = Exponent.GetExpression();

        return v => Mathf.Pow(termExp(v), exponentExp(v));
    }

    public override Equation GetDerivative(Variable wrt)
    {
        // Check for common cases
        if (Exponent is Constant)
        {
            if (Base is Constant)
            {
                // Both term and exponent are constant means this is constant
                return Constant.ZERO;
            }
            if (Exponent.Equals(Constant.ONE))
            {
                return Base.GetDerivative(wrt);
            }
            if (Exponent.Equals(Constant.ZERO))
            {
                return Constant.ZERO;
            }

            Equation baseDerivative = Base.GetDerivative(wrt);
            return Exponent * baseDerivative * Pow(Base, Exponent - 1);
        }

        // Check for common term cases
        if (Base.Equals(Constant.ONE))
        {
            return Constant.ZERO;
        }
        if (Base.Equals(Constant.ZERO))
        {
            return Constant.ZERO;
        }

        // Big derivative (u^v)'=(u^(v-1))(vu' + uv'ln(u))
        Equation termDeriv = Base.GetDerivative(wrt);
        Equation expDeriv = Exponent.GetDerivative(wrt);
        return Pow(Base, Exponent - 1) * ((Exponent * termDeriv) + (Base * expDeriv * new LnOperation(Base)));
    }

    public bool Equals(Exponentiation other)
    {
        if (other is null)
        {
            return false;
        }

        return Base.Equals(other.Base) && Exponent.Equals(other.Exponent);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Exponentiation);
    }

    public override int GetHashCode()
    {
        return 31 * Base.GetHashCode() + Exponent.GetHashCode();
    }

    public static bool operator ==(Exponentiation left, Exponentiation right)
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

    public static bool operator !=(Exponentiation left, Exponentiation right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return $"({Base} ^ {Exponent})";
    }

    public override Equation GetSimplified()
    {
        Equation newTerm = Base.GetSimplified();
        Equation newExponent = Exponent.GetSimplified();

        if (newExponent.Equals(Constant.ONE))
        {
            return newTerm;
        }

        if (newTerm is Constant termConstant && newExponent is Constant exponentConstant)
        {
            Rational numerator = exponentConstant.GetValue().Numerator;
            Rational denominator = exponentConstant.GetValue().Denominator;
            if (numerator < int.MaxValue && numerator > int.MinValue
                && denominator < int.MaxValue && numerator > int.MinValue)
            {
                Rational value = Rational.Pow(termConstant.GetValue(), (int)numerator);
                value = Rational.RationalRoot(value, (int)denominator);
                return new Constant(value).GetSimplified();
            }
        }

        if (newTerm.Equals(Base) && newExponent.Equals(Exponent))
        {
            return this;
        }

        return new Exponentiation(newTerm, newExponent).GetSimplified();
    }
}
