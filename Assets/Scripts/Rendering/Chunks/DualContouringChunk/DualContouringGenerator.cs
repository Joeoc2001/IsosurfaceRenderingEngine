using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace SDFRendering.Chunks.SurfaceNetChunk
{
    public class DualContouringGenerator : DualGenerator
    {
        public static readonly DualContouringGenerator Instance = new DualContouringGenerator();

        // Has no constant term as constant term can be ignored when finding minimum
        private class Unbounded3DPolynomial
        {
            private readonly float[] _coefficients = new float[9];

            public float XSquared {
                get => _coefficients[0];
                private set => _coefficients[0] = value;
            }
            public float YSquared
            {
                get => _coefficients[1];
                private set => _coefficients[1] = value;
            }
            public float ZSquared
            {
                get => _coefficients[2];
                private set => _coefficients[2] = value;
            }
            public float XY
            {
                get => _coefficients[3];
                private set => _coefficients[3] = value;
            }
            public float XZ
            {
                get => _coefficients[4];
                private set => _coefficients[4] = value;
            }
            public float YZ
            {
                get => _coefficients[5];
                private set => _coefficients[5] = value;
            }
            public float X
            {
                get => _coefficients[6];
                private set => _coefficients[6] = value;
            }
            public float Y
            {
                get => _coefficients[7];
                private set => _coefficients[7] = value;
            }
            public float Z
            {
                get => _coefficients[8];
                private set => _coefficients[8] = value;
            }

            public static Unbounded3DPolynomial operator +(Unbounded3DPolynomial a, Unbounded3DPolynomial b)
            {
                Unbounded3DPolynomial n = new Unbounded3DPolynomial();
                for (int i = 0; i < n._coefficients.Length; i++)
                {
                    n._coefficients[i] = a._coefficients[i] + b._coefficients[i];
                }
                return n;
            }

            public Unbounded3DPolynomial AddPlanarWeighting(Vector3 position, Vector3 normal)
            {
                normal = normal.normalized;

                float c = normal.x * position.x + normal.y * position.y + normal.z * position.z;
                Unbounded3DPolynomial newPoly = new Unbounded3DPolynomial
                {
                    XSquared = normal.x * normal.x,
                    YSquared = normal.y * normal.y,
                    ZSquared = normal.z * normal.z,

                    XY = 2 * normal.x * normal.y,
                    XZ = 2 * normal.x * normal.z,
                    YZ = 2 * normal.y * normal.z,

                    X = -2 * c * normal.x,
                    Y = -2 * c * normal.y,
                    Z = -2 * c * normal.z
                };


                return this + newPoly;
            }
        }

        private DualContouringGenerator() : base(0, 1, 1)
        {

        }

        protected override Vector3 CalculateVertex(FeelerNodeSet nodes, ImplicitSurface surface, Vector3Int index)
        {
            (Vector3Int, Vector3Int)[] edges = new (Vector3Int, Vector3Int)[]
            {
                ( new Vector3Int(0, 0, 0), new Vector3Int(0, 0, 1) ),
                ( new Vector3Int(0, 0, 0), new Vector3Int(0, 1, 0) ),
                ( new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0) ),
                ( new Vector3Int(0, 0, 1), new Vector3Int(0, 1, 1) ),
                ( new Vector3Int(0, 0, 1), new Vector3Int(1, 0, 1) ),
                ( new Vector3Int(0, 1, 0), new Vector3Int(0, 1, 1) ),
                ( new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0) ),
                ( new Vector3Int(1, 0, 0), new Vector3Int(1, 0, 1) ),
                ( new Vector3Int(1, 0, 0), new Vector3Int(1, 1, 0) ),
                ( new Vector3Int(0, 1, 1), new Vector3Int(1, 1, 1) ),
                ( new Vector3Int(1, 0, 1), new Vector3Int(1, 1, 1) ),
                ( new Vector3Int(1, 1, 0), new Vector3Int(1, 1, 1) ),
            };

            int count = 0;
            Vector3 total = Vector3.zero;
            foreach ((Vector3Int a, Vector3Int b) in edges)
            {
                FeelerNode nodeA = nodes[index + a];
                FeelerNode nodeB = nodes[index + b];

                if (nodeA.SignBit == nodeB.SignBit)
                {
                    continue;
                }

                float valA = nodeA.Val;
                float valB = nodeB.Val;
                float dist = valB / (valB - valA);
                Vector3 pos = dist * nodeA.Pos + (1 - dist) * nodeB.Pos;

                total += pos;
                count += 1;
            }

            return total / count;
        }
    }
}
