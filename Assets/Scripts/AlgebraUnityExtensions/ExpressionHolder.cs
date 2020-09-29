using Algebra;
using Algebra.Parsing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlgebraExtensions
{
    public class ExpressionHolder : ExpressionProvider
    {
        private Expression _expression;

        public string defaultExpression = "0";

        private void Start()
        {
            SetExpression(Parser.Parse(defaultExpression));
        }

        public void SetExpression(Expression expression)
        {
            this._expression = expression;
            ExpressionHasChanged(expression);
        }

        public override Expression GetExpression()
        {
            return _expression;
        }
    }
}
