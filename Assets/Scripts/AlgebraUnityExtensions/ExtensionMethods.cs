using Algebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AlgebraUnityExtensions
{
    public static class ExtensionMethods
    {
        public static VariableInputSet<double> GetInputs(Vector3 vector)
        {
            return new VariableInputSet<double>()
            {
                { "x", vector.x },
                { "y", vector.y },
                { "z", vector.z },
            };
        }

        public static double EvaluateOnce(this Expression expression, Vector2 values) => expression.EvaluateOnce(new VariableInputSet<double>() { { "x", values.x }, { "y", values.y } });
        public static double EvaluateOnce(this Expression expression, Vector3 values) => expression.EvaluateOnce(new VariableInputSet<double>() { { "x", values.x }, { "y", values.y }, { "z", values.z } });
        public static double EvaluateOnce(this Expression expression, Vector4 values) => expression.EvaluateOnce(new VariableInputSet<double>() { { "x", values.x }, { "y", values.y }, { "z", values.z }, { "w", values.w } });
    }
}
