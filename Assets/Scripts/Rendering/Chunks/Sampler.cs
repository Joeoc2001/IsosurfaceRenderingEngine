using Algebra;
using Algebra.Functions;
using AlgebraUnityExtensions;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SDFRendering.Chunks
{
    public class Sampler
    {
        public static PointCloud SampleGridAsync(Expression expression, int sideResolution, float sideSpacing, Vector3 origin)
        {
            return SampleGridAsync(expression, new Vector3Int(sideResolution, sideResolution, sideResolution), new Vector3(sideSpacing, sideSpacing, sideSpacing), origin);
        }

        public static PointCloud SampleGridAsync(Expression expression, Vector3Int resolution, Vector3 spacing, Vector3 origin)
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

            return new PointCloud(nodes, origin, spacing);
        }
    }
}
