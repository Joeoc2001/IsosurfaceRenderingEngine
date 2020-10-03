using Algebra;
using Algebra.Functions;
using AlgebraUnityExtensions;
using System;
using UnityEngine;

namespace SDFRendering.Chunks
{
    public class Sampler
    {
        public static void SampleGridAsync(Expression expression, Action<PointCloud> consumer, int sideResolution, float sideSpacing, Vector3 origin)
        {
            SampleGridAsync(expression, consumer, new Vector3Int(sideResolution, sideResolution, sideResolution), new Vector3(sideSpacing, sideSpacing, sideSpacing), origin);
        }

        public static void SampleGridAsync(Expression expression, Action<PointCloud> consumer, Vector3Int resolution, Vector3 spacing, Vector3 origin)
        {
            float[,,] nodes = new float[resolution.x, resolution.y, resolution.z];
            for (int x = 0; x < resolution.x; x++)
            {
                for (int y = 0; y < resolution.y; y++)
                {
                    for (int z = 0; z < resolution.z; z++)
                    {
                        // Construct position
                        Vector3 offset = new Vector3(x, y, z);
                        offset.Scale(spacing);
                        Vector3 position = origin + offset;

                        // Get value
                        VariableInputSet<double> variableSet = ExtensionMethods.GetInputs(position);
                        float value = (float)expression.EvaluateOnce(variableSet);

                        // Guard for invalid values
                        value = float.IsNaN(value) ? float.PositiveInfinity : value;

                        // Place in target array
                        nodes[x, y, z] = value;
                    }
                }
            }

            PointCloud pointCloud = new PointCloud(nodes, origin, spacing);
            consumer(pointCloud);
        }
    }
}
