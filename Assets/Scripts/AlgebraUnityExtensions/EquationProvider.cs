using Algebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace AlgebraExtensions
{
    public abstract class EquationProvider : MonoBehaviour
    {
        public delegate void EquationChangeEvent(EquationProvider provider, Equation equation);
        public event EquationChangeEvent OnEquationChange;

        protected void EquationHasChanged(Equation equation)
        {
            OnEquationChange.Invoke(this, equation);
        }

        public abstract Equation GetEquation();
    }
}