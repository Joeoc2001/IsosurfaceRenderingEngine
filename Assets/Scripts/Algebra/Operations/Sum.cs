using Rationals;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Algebra.Operations
{
    public class Sum : CommutativeOperation, IEquatable<Sum>
    {
        public static Equation Add<T>(List<T> eqs) where T : Equation
        {
            // Loop and find all other addition nodes and put them into this one
            List<Equation> collatedEqs = new List<Equation>();
            foreach (Equation eq in eqs)
            {
                if (eq is Sum addeq)
                {
                    collatedEqs.AddRange(addeq.Arguments);
                }
                else
                {
                    collatedEqs.Add(eq);
                }
            }

            // Put all of the constants together, and other generic commutative operations
            List<Equation> newEqs = SimplifyArguments(collatedEqs, 0, (x, y) => x + y);

            if (newEqs.Count() == 0)
            {
                return 0;
            }
            if (newEqs.Count() == 1)
            {
                return newEqs[0];
            }

            // Collate Multiplication terms
            Dictionary<Equation, Constant> terms = new Dictionary<Equation, Constant>();
            foreach (Equation eq in newEqs)
            {
                Equation baseEq;
                Constant newCoefficient;
                if (eq is Product multeq)
                {
                    baseEq = multeq.GetVariable();
                    newCoefficient = multeq.GetConstantCoefficient();
                }
                else
                {
                    baseEq = eq;
                    newCoefficient = 1;
                }

                if (terms.TryGetValue(baseEq, out Constant coefficient))
                {
                    newCoefficient = Constant.From(newCoefficient.GetValue() + coefficient.GetValue());
                }
                terms[baseEq] = newCoefficient;
            }
            // Put back into exponent form
            newEqs.Clear();
            foreach (Equation eq in terms.Keys)
            {
                Equation newEq = eq * terms[eq];

                if (newEq.Equals(Constant.ZERO))
                {
                    continue;
                }

                newEqs.Add(newEq);
            }

            if (newEqs.Count() == 0)
            {
                return 0;
            }
            if (newEqs.Count() == 1)
            {
                return newEqs[0];
            }

            return new Sum(newEqs);
        }

        private Sum(IList<Equation> eqs)
            : base(eqs)
        {

        }

        public override Equation GetDerivative(Variable wrt)
        {
            List<Equation> derivatives = new List<Equation>();
            foreach (Equation e in Arguments)
            {
                derivatives.Add(e.GetDerivative(wrt));
            }
            return Add(derivatives);
        }

        public bool Equals(Sum obj)
        {
            if (obj is null)
            {
                return false;
            }

            // Check for commutativity
            return OperandsEquals(obj.Arguments);
        }

        public override bool Equals(Equation obj)
        {
            return this.Equals(obj as Sum);
        }

        public override int GenHashCode()
        {
            return base.GenHashCode() ^ -1375070008;
        }

        public override int IdentityValue()
        {
            return 0;
        }

        public override float Perform(float a, float b)
        {
            return a + b;
        }

        public override string EmptyName()
        {
            return "[EMPTY SUM]";
        }

        public override string OperationSymbol()
        {
            return "+";
        }

        public override int GetOrderIndex()
        {
            return 30;
        }

        public override Func<List<Equation>, Equation> GetSimplifyingConstructor()
        {
            return Add;
        }
    }
}