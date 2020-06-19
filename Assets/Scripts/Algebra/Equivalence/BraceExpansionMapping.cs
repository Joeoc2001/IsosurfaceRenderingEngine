using Algebra.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algebra.Equivalence
{
    internal class BraceExpansionMapping : EquationMapping
    {
        private int index;

        public BraceExpansionMapping(int i)
        {
            this.index = i;

            PostMap = map;
            ShouldMapChildren = shouldMapChildren;
            ShouldMapThis = shouldMapThis;
        }

        private Equation map(Equation e)
        {
            if (!(e is Product p))
            {
                return e;
            }

            // Extract all sum terms from the product
            List<Sum> sums = new List<Sum>();
            List<Equation> others = new List<Equation>();
            foreach (Equation arg in p.Arguments)
            {
                if (arg is Sum s)
                {
                    sums.Add(s);
                }
                else
                {
                    others.Add(arg);
                }
            }

            // Loop through pairs to find where i is 0
            for (int j = 0; j < sums.Count - 1; j++)
            {
                for (int k = j + 1; k < sums.Count; k++)
                {
                    if (index-- == 0)
                    {
                        // Extract the two sum terms
                        Sum s1 = sums[j];
                        Sum s2 = sums[k];
                        sums.RemoveAt(k);
                        sums.RemoveAt(j);

                        // Multiply them
                        Equation newTerm = ExpandSums(s1, s2);
                        others.Add(newTerm);

                        // Create a new product
                        others.AddRange(sums);
                        return Product.Multiply(others);
                    }
                }
            }

            // If i didn't hit 0 then the pair isn't in this multiplication
            return e;
        }

        private bool shouldMapThis(Equation e) => e is Product && index >= 0;
        private bool shouldMapChildren(Equation e) => index >= 0;

        private static Equation ExpandSums(Sum s1, Sum s2)
        {
            List<Equation> terms = new List<Equation>();
            foreach (Equation t1 in s1.Arguments)
            {
                foreach (Equation t2 in s2.Arguments)
                {
                    terms.Add(t1 * t2);
                }
            }
            return Sum.Add(terms);
        }
    }
}
