using Algebra;
using Algebra.Parsing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlgebraExtensions
{
    public class EquationHolder : EquationProvider
    {
        private Equation equation;

        public string defaultEquation = "0";

        private void Start()
        {
            SetEquation(Parser.Parse(defaultEquation));
        }

        public void SetEquation(Equation equation)
        {
            this.equation = equation;
            EquationHasChanged(equation);
        }

        public override Equation GetEquation()
        {
            return equation;
        }
    }
}
