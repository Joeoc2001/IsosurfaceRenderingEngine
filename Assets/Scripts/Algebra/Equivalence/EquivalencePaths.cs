using Algebra.Operations;
using System.Collections.Generic;

namespace Algebra.Equivalence
{
    public static class EquivalencePaths
    {
        public static readonly EquivalencePath IDENTITY = eq => new List<Equation> { eq };

        public static readonly EquivalencePath EXPAND_BRACES = eq =>
        {
            int i = 0;
            List<Equation> newEqs = new List<Equation>();
            while (true)
            {
                EquationMapping expansionMapping = new BraceExpansionMapping(i);

                Equation newEq = eq.Map(expansionMapping);

                if (newEq.Equals(eq))
                {
                    break;
                }

                newEqs.Add(newEq);
                i++;
            }
            return newEqs;
        };
    }
}