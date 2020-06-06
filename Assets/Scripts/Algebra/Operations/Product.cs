using Rationals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

class Product : CommutativeOperation, IEquatable<Product>
{
    public static Equation Multiply<T>(List<T> eqs) where T : Equation
    {
        // Collate multiplications into one big multiplication
        List<Equation> collatedEqs = new List<Equation>();
        foreach (Equation eq in eqs)
        {
            if (eq is Product multeq)
            {
                collatedEqs.AddRange(multeq.eqs);
                continue;
            }

            collatedEqs.Add(eq);
        }

        List<Equation> newEqs = SimplifyArguments(collatedEqs, 1, (x, y) => x * y);

        if (newEqs.Count == 0)
        {
            return 1;
        }
        if (newEqs.Count == 1)
        {
            return newEqs[0];
        }

        foreach (Equation eq in newEqs)
        {
            if (eq.Equals(Constant.ZERO))
            {
                return 0;
            }
        }

        // Collate exponents
        Dictionary<Equation, List<Equation>> exponents = new Dictionary<Equation, List<Equation>>();
        foreach (Equation eq in newEqs)
        {
            Equation baseEq;
            Equation exponentEq;
            if (eq is Exponent expeq)
            {
                baseEq = expeq.Base;
                exponentEq = expeq.Power;
            }
            else
            {
                baseEq = eq;
                exponentEq = 1;
            }

            if (!exponents.ContainsKey(baseEq))
            {
                exponents.Add(baseEq, new List<Equation>());
            }
            exponents[baseEq].Add(exponentEq);
        }
        // Put back into exponent form
        newEqs.Clear();
        foreach (Equation eq in exponents.Keys)
        {
            List<Equation> powers = exponents[eq];

            newEqs.Add(Pow(eq, Add(powers)));
        }

        if (newEqs.Count == 1)
        {
            return newEqs[0];
        }

        return new Product(newEqs);
    }

    private Product(IEnumerable<Equation> eqs)
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
            terms.Add(Multiply(term));
        }
        return Sum.Add(terms);
    }

    public bool Equals(Product obj)
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
        return this.Equals(obj as Product);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode() ^ -914308772;
    }

    public override int IdentityValue()
    {
        return 1;
    }

    public override float Perform(float a, float b)
    {
        return a * b;
    }

    public override string EmptyName()
    {
        return "[EMPTY PRODUCT]";
    }

    public override string OperationSymbol()
    {
        return "*";
    }

    // Finds the first constant in the multiplication, or returns 1 if there are none
    public Equation GetConstantCoefficient()
    {
        foreach (Equation eq in eqs)
        {
            if (eq is Constant c)
            {
                return c;
            }
        }
        return 1;
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
                return new Product(others);
            }
        }
        return this;
    }

    public override string OperationName()
    {
        return "MULTIPLICATION";
    }

    public static bool operator ==(Product left, Product right)
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

    public static bool operator !=(Product left, Product right)
    {
        return !(left == right);
    }

    public override int GetOrderIndex()
    {
        return 20;
    }

    public override int CompareTo(Equation other)
    {
        throw new NotImplementedException();
    }
}
