using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rationals;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class EquationTests
    {
        public static Equation GenerateRandomEquation(float baseProb, int maxDepth)
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
                    return Equation.LnOf(GenerateRandomEquation(baseProb, maxDepth - 1));
                }
                else if (functionValue < 0.2)
                {
                    // Return a sign function
                    return Equation.SignOf(GenerateRandomEquation(baseProb, maxDepth - 1));
                }
                else if (functionValue < 0.3)
                {
                    // Return a min function
                    return Equation.Min(GenerateRandomEquation(baseProb, maxDepth - 3),
                        GenerateRandomEquation(baseProb, maxDepth - 3));
                }
                else if (functionValue < 0.4)
                {
                    // Return a max function
                    return Equation.Max(GenerateRandomEquation(baseProb, maxDepth - 3),
                        GenerateRandomEquation(baseProb, maxDepth - 3));
                }
                else if (functionValue < 0.5)
                {
                    // Return an Abs function
                    return Equation.Abs(GenerateRandomEquation(baseProb, maxDepth - 2));
                }
                else
                {
                    // Return exponentiation
                    return Equation.Pow(GenerateRandomEquation(baseProb, maxDepth - 1),
                        GenerateRandomEquation(baseProb, maxDepth - 1));
                }
            }
            else // Addition or subtraction
            {
                int x = Random.Range(1, 5);
                List<Equation> eqs = new List<Equation>(x);
                for (int i = 0; i < x; i++)
                {
                    eqs.Add(GenerateRandomEquation(baseProb, maxDepth - 1));
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
