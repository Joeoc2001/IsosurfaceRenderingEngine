using Rationals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract class CommutativeOperation : Equation
{
    protected readonly List<Equation> eqs;

    public CommutativeOperation(IEnumerable<Equation> eqs)
    {
        this.eqs = new List<Equation>(eqs);
        this.eqs.Sort((x, y) => x.GetHashCode().CompareTo(y.GetHashCode()));
    }

    public abstract int IdentityValue();
    public abstract float Operation(float a, float b);
    public abstract Rational Operation(Rational a, Rational b);
    public abstract string EmptyName();
    public abstract string OperationName();

    public override sealed Func<VariableSet, float> GetExpression()
    {
        List<Func<VariableSet, float>> expressions = new List<Func<VariableSet, float>>(eqs.Count());
        foreach (Equation e in eqs)
        {
            expressions.Add(e.GetExpression());
        }

        return v =>
        {
            float value = IdentityValue();
            foreach (Func<VariableSet, float> f in expressions)
            {
                value = Operation(value, f(v));
            }
            return value;
        };
    }

    public bool OperandsEquals(List<Equation> operands)
    {
        // Check for commutativity
        var counts = eqs
            .GroupBy(v => v)
            .ToDictionary(g => g.Key, g => g.Count());
        var ok = true;
        foreach (Equation n in operands)
        {
            if (counts.TryGetValue(n, out int c))
            {
                counts[n] = c - 1;
            }
            else
            {
                ok = false;
                break;
            }
        }
        return ok && counts.Values.All(c => c == 0);
    }

    protected List<Equation> SimplifyArguments()
    {
        List<Equation> newEqs = new List<Equation>();

        Constant identityConstant = new Constant(IdentityValue());

        Rational collectedConstants = IdentityValue();

        // Loop & simplify
        foreach (Equation eq in eqs)
        {
            Equation newEq = eq.GetSimplified();

            if (newEq.Equals(identityConstant))
            {
                continue;
            }

            if (eq is Constant constEq)
            {
                collectedConstants = Operation(collectedConstants, constEq.GetValue());
                continue;
            }

            newEqs.Add(newEq);
        }

        if (!collectedConstants.Equals(IdentityValue()))
        {
            newEqs.Add(new Constant(collectedConstants));
        }

        return newEqs;
    }

    public override string ToString()
    {
        if (eqs.Count == 0)
        {
            return EmptyName();
        }

        StringBuilder builder = new StringBuilder("(");
        builder.Append(eqs[0].ToString());

        for (int i = 1; i < eqs.Count; i++)
        {
            builder.Append(" ");
            builder.Append(OperationName());
            builder.Append(" ");
            builder.Append(eqs[i].ToString());
        }

        builder.Append(")");

        return builder.ToString();
    }

    public override int GetHashCode()
    {
        int value = -1906136416 ^ OperationName().GetHashCode();
        foreach (Equation eq in eqs)
        {
            value ^= eq.GetHashCode();
        }
        return value;
    }
}
