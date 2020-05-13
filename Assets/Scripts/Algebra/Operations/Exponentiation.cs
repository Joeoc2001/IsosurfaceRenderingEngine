using Rationals;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Exponentiation : Equation, IEquatable<Exponentiation>
{
    public readonly Equation Base;
    public readonly Equation Exponent;

    new public static Equation Pow(Equation term, Equation exponent)
    {
        if (exponent.Equals(Constant.ZERO))
        {
            return 1;
        }

        if (exponent.Equals(Constant.ONE))
        {
            return term;
        }

        if (term is Constant termConstant && exponent is Constant exponentConstant)
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

        return new Exponentiation(term, exponent);
    }

    public Exponentiation(Equation term, Equation exponent)
    {
        this.Base = term;
        this.Exponent = exponent;
    }

    public override ExpressionDelegate GetExpression()
    {
        ExpressionDelegate termExp = Base.GetExpression();

        if (Exponent.Equals(-1))
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
            Equation baseDerivative = Base.GetDerivative(wrt);
            return Exponent * baseDerivative * Pow(Base, ((Constant)Exponent).GetValue() - 1);
        }

        if (Base is Constant)
        {
            Equation exponentDerivative = Exponent.GetDerivative(wrt);
            return Ln(Base) * exponentDerivative * this;
        }

        // Big derivative (u^v)'=(u^v)(vu'/u + v'ln(u))
        // Alternatively  (u^v)'=(u^(v-1))(vu' + uv'ln(u)) but I find the first form simplifies faster
        Equation baseDeriv = Base.GetDerivative(wrt);
        Equation expDeriv = Exponent.GetDerivative(wrt);
        return this * ((Exponent * baseDeriv / Base) + (expDeriv * Ln(Base)));
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
        return $"[EXPONENTIATION]({Base}, {Exponent})";
    }

    public override string ToParsableString()
    {
        return $"({Base.ToParsableString()} ^ {Exponent.ToParsableString()})";
    }

    public override string ToRunnableString()
    {
        return $"Equation.Pow({Base.ToRunnableString()}, {Exponent.ToRunnableString()})";
    }
}
