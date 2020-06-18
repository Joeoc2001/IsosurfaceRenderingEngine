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
    }

    public abstract int IdentityValue();
    public abstract float Perform(float a, float b);
    public delegate Rational Operation(Rational a, Rational b);
    public abstract string OperationName();
    public abstract string EmptyName();
    public abstract string OperationSymbol();
    public abstract Func<List<Equation>, Equation> GetSimplifyingConstructor();

    public override sealed ExpressionDelegate GetExpression()
    {
        List<ExpressionDelegate> expressions = new List<ExpressionDelegate>(eqs.Count());
        foreach (Equation e in eqs)
        {
            expressions.Add(e.GetExpression());
        }

        float identityValue = IdentityValue();

        return v =>
        {
            float value = identityValue;
            foreach (ExpressionDelegate f in expressions)
            {
                value = Perform(value, f(v));
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

    public List<Equation> GetDisplaySortedArguments()
    {
        List<Equation> sortedEqs = new List<Equation>(eqs);
        sortedEqs.Sort(EquationDisplayComparer.COMPARER);
        return sortedEqs;
    }

    protected static List<Equation> SimplifyArguments<T>(List<T> eqs, Rational identity, Operation operation) where T : Equation
    {
        List<Equation> newEqs = new List<Equation>(eqs.Count);

        Rational collectedConstants = identity;

        // Loop & simplify
        foreach (Equation eq in eqs)
        {
            if (eq is Constant constEq)
            {
                collectedConstants = operation(collectedConstants, constEq.GetValue());
                continue;
            }

            newEqs.Add(eq);
        }

        if (!collectedConstants.Equals(identity))
        {
            newEqs.Add(Constant.From(collectedConstants));
        }

        return newEqs;
    }

    public override int GetHashCode()
    {
        int value = -1906136416 ^ OperationSymbol().GetHashCode();
        foreach (Equation eq in eqs)
        {
            value ^= eq.GetHashCode();
        }
        return value;
    }

    public override string ToString()
    {
        if (eqs.Count == 0)
        {
            return EmptyName();
        }

        List<Equation> sortedEquations = GetDisplaySortedArguments();

        StringBuilder builder = new StringBuilder($"[{OperationName()}](");
        builder.Append(sortedEquations[0].ToString());

        for (int i = 1; i < sortedEquations.Count; i++)
        {
            builder.Append(", ");
            builder.Append(sortedEquations[i].ToString());
        }

        builder.Append(")");

        return builder.ToString();
    }

    public override string ToParsableString()
    {
        if (eqs.Count == 0)
        {
            return "()";
        }

        StringBuilder builder = new StringBuilder();

        builder.Append(ParenthesisedParsableString(eqs[0]));
        for (int i = 1; i < eqs.Count; i++)
        {
            builder.Append(" ");
            builder.Append(OperationSymbol());
            builder.Append(" ");
            builder.Append(ParenthesisedParsableString(eqs[i]));
        }

        return builder.ToString();
    }

    public override string ToRunnableString()
    {
        if (eqs.Count == 0)
        {
            return EmptyName();
        }

        StringBuilder builder = new StringBuilder("(");
        builder.Append(eqs[0].ToRunnableString());

        for (int i = 1; i < eqs.Count; i++)
        {
            builder.Append(" ");
            builder.Append(OperationSymbol());
            builder.Append(" ");
            builder.Append(eqs[i].ToRunnableString());
        }

        builder.Append(")");

        return builder.ToString();
    }

    public override Equation Map(EquationMapping map)
    {
        Equation currentThis = this;

        if (map.ShouldMapChildren(this))
        {
            List<Equation> mappedEqs = new List<Equation>(eqs.Count);

            foreach (Equation eq in eqs)
            {
                mappedEqs.Add(eq.Map(map));
            }

            currentThis = GetSimplifyingConstructor()(mappedEqs);
        }

        if (map.ShouldMapThis(this))
        {
            currentThis = map.PostMap(currentThis);
        }

        return currentThis;
    }
}
