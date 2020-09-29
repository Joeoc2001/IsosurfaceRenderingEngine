using Algebra;
using Algebra.Functions;
using AlgebraUnityExtensions;
using System;
using UnityEngine;

namespace SDFRendering.Chunks
{
    public class Sampler
    {
        public static void SampleGridAsync(Expression expression, Action<FeelerNodeSet> consumer, int resolution, float delta, Vector3 origin, Vector3 samplingOffset)
        {
            FeelerNode[] nodes = new FeelerNode[resolution * resolution * resolution];
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    for (int z = 0; z < resolution; z++)
                    {
                        // Construct position
                        Vector3 position = origin + (delta * new Vector3(x, y, z));

                        // Get value
                        VariableInputSet<double> variableSet = ExtensionMethods.GetInputs(position + samplingOffset);
                        float value = (float)expression.EvaluateOnce(variableSet);

                        // Guard for invalid values
                        value = float.IsNaN(value) ? 0 : value;

                        // Place in target array
                        int index = (x * resolution + y) * resolution + z;
                        nodes[index] = new FeelerNode(position, value);
                    }
                }
            }

            FeelerNodeSet pointCloud = new FeelerNodeSet(resolution, nodes);
            consumer(pointCloud);
        }
    }
}
