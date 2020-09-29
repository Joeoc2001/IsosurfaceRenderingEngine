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
    }
}
