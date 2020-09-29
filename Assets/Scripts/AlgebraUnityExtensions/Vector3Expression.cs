using Algebra;
using System;
using UnityEngine;

namespace AlgebraExtensions
{
    public class Vector3Expression
    {
        private readonly Expression _x;
        private readonly Expression _y;
        private readonly Expression _z;

        public Vector3Expression(Expression x, Expression y, Expression z)
        {
            _x = x ?? throw new ArgumentNullException(nameof(x));
            _y = y ?? throw new ArgumentNullException(nameof(y));
            _z = z ?? throw new ArgumentNullException(nameof(z));
        }

        public static Vector3Expression GetFromGradient(Expression e, string a="x", string b="y", string c="z")
        {
            return new Vector3Expression(
                    e.GetDerivative(a),
                    e.GetDerivative(b),
                    e.GetDerivative(c)
                );
        }

        public Vector3 EvaluateOnce(VariableInputSet<double> variableInputs)
        {
            return new Vector3((float)_x.EvaluateOnce(variableInputs),
                               (float)_y.EvaluateOnce(variableInputs),
                               (float)_z.EvaluateOnce(variableInputs));
        }
    }
}
