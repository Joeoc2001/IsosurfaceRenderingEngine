using Rationals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Algebra.Operations
{
    public abstract class CommutativeOperation : Equation
    {
        public readonly ReadOnlyCollection<Equation> Arguments;

        public CommutativeOperation(IList<Equation> eqs)
        {
            this.Arguments = new ReadOnlyCollection<Equation>(eqs);
        }

        public abstract int IdentityValue();
        public abstract float Perform(float a, float b);
        public delegate Rational Operation(Rational a, Rational b);
        public abstract string EmptyName();
        public abstract string OperationSymbol();
        public abstract Func<List<Equation>, Equation> GetSimplifyingConstructor();

        public override sealed ExpressionDelegate GetExpression()
        {
            List<ExpressionDelegate> expressions = new List<ExpressionDelegate>(Arguments.Count());
            foreach (Equation e in Arguments)
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

        public bool OperandsEquals(IList<Equation> operands)
        {
            // Check for commutativity
            var counts = Arguments
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
            List<Equation> sortedEqs = new List<Equation>(Arguments);
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

        public override int GenHashCode()
        {
            int value = -1906136416 ^ OperationSymbol().GetHashCode();
            foreach (Equation eq in GetDisplaySortedArguments())
            {
                value *= 33;
                value ^= eq.GenHashCode();
            }
            return value;
        }

        public override string ToString()
        {
            if (Arguments.Count == 0)
            {
                return "()";
            }

            StringBuilder builder = new StringBuilder();

            builder.Append(ToParenthesisedString(Arguments[0]));
            for (int i = 1; i < Arguments.Count; i++)
            {
                builder.Append(" ");
                builder.Append(OperationSymbol());
                builder.Append(" ");
                builder.Append(ToParenthesisedString(Arguments[i]));
            }

            return builder.ToString();
        }

        public override string ToRunnableString()
        {
            if (Arguments.Count == 0)
            {
                return EmptyName();
            }

            StringBuilder builder = new StringBuilder("(");
            builder.Append(Arguments[0].ToRunnableString());

            for (int i = 1; i < Arguments.Count; i++)
            {
                builder.Append(" ");
                builder.Append(OperationSymbol());
                builder.Append(" ");
                builder.Append(Arguments[i].ToRunnableString());
            }

            builder.Append(")");

            return builder.ToString();
        }

        public override Equation Map(EquationMapping map)
        {
            Equation currentThis = this;

            if (map.ShouldMapChildren(this))
            {
                List<Equation> mappedEqs = new List<Equation>(Arguments.Count);

                foreach (Equation eq in Arguments)
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
}