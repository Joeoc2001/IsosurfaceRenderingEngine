using Rationals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class Multiplication : CommutativeOperation, IEquatable<Multiplication>
{
    public Multiplication(IEnumerable<Equation> eqs)
        : base(eqs)
    {

    }

    public override Equation GetDerivative(Variable wrt)
    {
        // Get all derivatives
        List<Equation> derivatives = new List<Equation>(eqs.Count);
        foreach (Equation eq in eqs)
        {
            derivatives.Add(eq.GetDerivative(wrt));
        }

        // Collate into multi term product rule
        List<Equation> terms = new List<Equation>();
        for (int iDerivative = 0; iDerivative < eqs.Count; iDerivative++)
        {
            List<Equation> term = new List<Equation>()
            {
                derivatives[iDerivative]
            };
            for (int iCoefficient = 0; iCoefficient < eqs.Count; iCoefficient++)
            {
                if (iCoefficient == iDerivative)
                {
                    continue;
                }

                term.Add(eqs[iCoefficient]);
            }
            terms.Add(new Multiplication(term));
        }
        return new Addition(terms);
    }

    public bool Equals(Multiplication obj)
    {
        if (obj is null)
        {
            return false;
        }

        // Check for commutativity
        return OperandsEquals(obj.eqs);
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as Multiplication);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override int IdentityValue()
    {
        return 1;
    }

    public override float Operation(float a, float b)
    {
        return a * b;
    }

    public override Rational Operation(Rational a, Rational b)
    {
        return a * b;
    }

    public override string EmptyName()
    {
        return "[EMPTY PRODUCT]";
    }

    public override string OperationName()
    {
        return "*";
    }

    public override Equation GetSimplified()
    {
        List<Equation> newEqs = SimplifyArguments();

        if (newEqs.Count == 0)
        {
            return Constant.ONE;
        }

        if (newEqs.Count == 1)
        {
            return newEqs[0];
        }

        foreach (Equation eq in newEqs)
        {
            if (eq.Equals(Constant.ZERO))
            {
                return Constant.ZERO;
            }
        }

        // Collate multiplications into one big multiplication
        List<Equation> collatedEqs = new List<Equation>();
        foreach (Equation eq in newEqs)
        {
            if (eq is Multiplication multeq)
            {
                collatedEqs.AddRange(multeq.eqs);
                continue;
            }

            collatedEqs.Add(eq);
        }

        // Collate exponents
        Dictionary<Equation, List<Equation>> exponents = new Dictionary<Equation, List<Equation>>();
        foreach (Equation eq in collatedEqs)
        {
            Equation baseEq;
            Equation exponentEq;
            if (eq is Exponentiation expeq)
            {
                baseEq = expeq.Base;
                exponentEq = expeq.Exponent;
            }
            else
            {
                baseEq = eq;
                exponentEq = Constant.ONE;
            }

            if (!exponents.ContainsKey(baseEq))
            {
                exponents.Add(baseEq, new List<Equation>());
            }
            exponents[baseEq].Add(exponentEq);
        }
        // Put back into exponent form
        collatedEqs.Clear();
        foreach (Equation eq in exponents.Keys)
        {
            List<Equation> powers = exponents[eq];

            Equation newTerm;
            if (powers.Count == 1)
            {
                if (powers[0] is Constant c && c.GetValue().Equals(1))
                {
                    newTerm = eq;
                }
                else
                {
                    newTerm = new Exponentiation(eq, powers[0]);
                }
            }
            else
            {
                newTerm = new Exponentiation(eq, new Addition(powers));
            }
            collatedEqs.Add(newTerm);
        }

        if (OperandsEquals(collatedEqs))
        {
            return this;
        }

        return new Multiplication(collatedEqs).GetSimplified();
    }

    // Finds the first constant in the multiplication, or returns 1 if there are none
    public Constant GetConstantCoefficient()
    {
        foreach (Equation eq in eqs)
        {
            if (eq is Constant c)
            {
                return c;
            }
        }
        return Constant.ONE;
    }

    // Gets a multiplication term of all of the terms minus the first constant, or this if there are no constants
    public Equation GetVariable()
    {
        foreach (Equation eq in eqs)
        {
            if (eq is Constant)
            {
                List<Equation> others = new List<Equation>(eqs);
                others.Remove(eq);
                if (others.Count == 1)
                {
                    return others[0];
                }
                return new Multiplication(others);
            }
        }
        return this;
    }

    public static bool operator ==(Multiplication left, Multiplication right)
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

    public static bool operator !=(Multiplication left, Multiplication right)
    {
        return !(left == right);
    }
}
