using Algebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace AlgebraExtensions
{
    public abstract class ExpressionProvider : MonoBehaviour
    {
        public delegate void ExpressionChangeEvent(ExpressionProvider provider, Expression equation);
        public event ExpressionChangeEvent OnExpressionChange;

        protected void ExpressionHasChanged(Expression equation)
        {
            OnExpressionChange.Invoke(this, equation);
        }

        public abstract Expression GetExpression();
    }
}