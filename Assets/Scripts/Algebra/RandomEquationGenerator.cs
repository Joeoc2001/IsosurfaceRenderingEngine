using Algebra.Operations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Algebra
{
    public class RandomEquationGenerator
    {
        public float baseProb;
        public int maxDepth;

        public Equation Next()
        {
            return Gen(baseProb, maxDepth);
        }

        private static Equation Gen(float baseProb, int maxDepth)
        {
            if (maxDepth <= 0 || Random.value < baseProb) // Return a terminating node
            {
                float terminatingValue = Random.value;
                if (terminatingValue < 0.7)
                {
                    // Return a constant
                    return Random.value;
                }
                else
                {
                    // Return a variable
                    var vars = Variable.VariableDict.Values;
                    return vars.ToList()[Random.Range(0, vars.Count)];
                }
            }

            if (Random.value < 0.7) // Return a simple function node
            {
                float functionValue = Random.value;
                if (functionValue < 0.1)
                {
                    // Return a log function
                    return Equation.LnOf(Gen(baseProb, maxDepth - 1));
                }
                else if (functionValue < 0.2)
                {
                    // Return a sign function
                    return Equation.SignOf(Gen(baseProb, maxDepth - 1));
                }
                else if (functionValue < 0.3)
                {
                    // Return a min function
                    return Equation.Min(Gen(baseProb, maxDepth - 1),
                        Gen(baseProb, maxDepth - 1));
                }
                else if (functionValue < 0.4)
                {
                    // Return a max function
                    return Equation.Max(Gen(baseProb, maxDepth - 1),
                        Gen(baseProb, maxDepth - 1));
                }
                else if (functionValue < 0.5)
                {
                    // Return an Abs function
                    return Equation.Abs(Gen(baseProb, maxDepth - 1));
                }
                else if (functionValue < 0.6)
                {
                    // Return an Sin function
                    return Equation.SinOf(Gen(baseProb, maxDepth - 1));
                }
                else if (functionValue < 0.7)
                {
                    // Return an Sin function
                    return Equation.CosOf(Gen(baseProb, maxDepth - 1));
                }
                else if (functionValue < 0.8)
                {
                    // Return an Sin function
                    return Equation.TanOf(Gen(baseProb, maxDepth - 1));
                }
                else
                {
                    // Return exponentiation
                    return Equation.Pow(Gen(baseProb, maxDepth - 1),
                        Gen(baseProb, maxDepth - 1));
                }
            }
            else // Addition or subtraction
            {
                int x = Random.Range(1, 5);
                List<Equation> eqs = new List<Equation>(x);
                for (int i = 0; i < x; i++)
                {
                    eqs.Add(Gen(baseProb, maxDepth - 1));
                }

                float operationValue = Random.value;
                if (operationValue < 0.5)
                {
                    // Return addition
                    return Equation.Add(eqs);
                }
                else
                {
                    // Return Multiplication
                    return Equation.Multiply(eqs);
                }
            }
        }
    }
}
