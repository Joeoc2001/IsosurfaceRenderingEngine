using Rationals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Addition : CommutativeOperation, IEquatable<Addition>
{
    public Addition(IEnumerable<Equation> eqs)
        : base(eqs)
    {

    }

    public override Equation GetDerivative(Variable wrt)
    {
        List<Equation> derivatives = new List<Equation>();
        foreach (Equation e in eqs)
        {
            derivatives.Add(e.GetDerivative(wrt));
        }
        return new Addition(derivatives);
    }

    public override Polynomial GetTaylorApproximation(VectorN origin, int order)
    {
        throw new NotImplementedException();
    }

    public bool Equals(Addition obj)
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
        return this.Equals(obj as Addition);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override int IdentityValue()
    {
        return 0;
    }

    public override float Operation(float a, float b)
    {
        return a + b;
    }

    public override string EmptyName()
    {
        return "[EMPTY SUM]";
    }

    public override string OperationName()
    {
        return "+";
    }

    public override Equation GetSimplified()
    {
        List<Equation> newEqs = SimplifyArguments();

        if (newEqs.Count == 0)
        {
            return Constant.ZERO;
        }

        if (newEqs.Count == 1)
        {
            return newEqs[0];
        }

        List<Equation> collatedEqs = new List<Equation>();
        foreach (Equation eq in newEqs)
        {
            if (eq is Addition addeq)
            {
                collatedEqs.AddRange(addeq.eqs);
            }
            else
            {
                collatedEqs.Add(eq);
            }
        }

        // Collate Multiplication terms
        Dictionary<Equation, List<Constant>> terms = new Dictionary<Equation, List<Constant>>();
        foreach (Equation eq in collatedEqs)
        {
            Equation baseEq;
            Constant coefficientEq;
            if (eq is Multiplication multeq)
            {
                baseEq = multeq.GetVariable();
                coefficientEq = multeq.GetConstantCoefficient();
            }
            else
            {
                baseEq = eq;
                coefficientEq = Constant.ONE;
            }

            if (!terms.ContainsKey(baseEq))
            {
                terms.Add(baseEq, new List<Constant>());
            }
            terms[baseEq].Add(coefficientEq);
        }
        // Put back into exponent form
        collatedEqs.Clear();
        foreach (Equation eq in terms.Keys)
        {
            List<Constant> coefficients = terms[eq];

            Equation newTerm;
            if (coefficients.Count == 1)
            {
                if (coefficients[0].GetValue().Equals(1))
                {
                    newTerm = eq;
                }
                else
                {
                newTerm = new Multiplication(new Equation[]{ eq, coefficients[0] });
                }
            }
            else
            {
                newTerm = new Multiplication(new Equation[] { eq, new Addition(coefficients) });
            }
            collatedEqs.Add(newTerm);
        }

        if (OperandsEquals(collatedEqs))
        {
            return this;
        }

        return new Addition(collatedEqs).GetSimplified();
    }

    public override Rational Operation(Rational a, Rational b)
    {
        return a + b;
    }

    public static bool operator==(Addition left, Addition right)
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

    public static bool operator !=(Addition left, Addition right)
    {
        return !(left == right);
    }
}
