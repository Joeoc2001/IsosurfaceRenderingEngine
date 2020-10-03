using System;
using System.Collections.Generic;
using UnityEngine;

namespace SDFRendering.Chunks.SurfaceNetChunk
{
    public class DualContouringGenerator : DualGenerator
    {
        public static readonly DualContouringGenerator Instance = new DualContouringGenerator();

        private DualContouringGenerator()
        {

        }

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

            public void Add(Unbounded3DPolynomial b)
            {
                for (int i = 0; i < _coefficients.Length; i++)
                {
                    _coefficients[i] += b._coefficients[i];
                }
            }

            public void AddPlanarWeighting(Vector3 position, Vector3 normal)
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

                Add(newPoly);
            }

            public Vector3 FindTurningPoint()
            {
                // this = Ax^2 + By^2 + Cz^2 + Dxy + Exz + Fyz + Gx + Hy + Iz
                // 
                //    | d this/dx |   |2Ax +  Dy +  Ez + G |   | 0 |
                // => | d this/dy | = | Dx + 2By +  Fz + H | = | 0 |
                //    | d this/dz |   | Ex +  Fy + 2Cz + I |   | 0 |
                //
                //    | x |   | 2A   D   E |-1     | -G |
                // => | y | = |  D  2B   F |   x   | -H |
                //    | z |   |  E   F  2C |       | -I |

                // Matrix constructor takes column vectors, so below is the matrix transposed
                // Not that it matters anyway
                Matrix4x4 matrix = new Matrix4x4(
                    new Vector4(2 * XSquared, XY, XZ, 0),
                    new Vector4(XY, 2 * YSquared, YZ, 0),
                    new Vector4(XZ, YZ, 2 * ZSquared, 0),
                    new Vector4(0,     0,     0,      1)
                );

                if (matrix.determinant == 0)
                {
                    throw new ArgumentOutOfRangeException("Determinant is 0 for given expression, so no minimum exists");
                }

                Vector4 ans = matrix.inverse * new Vector4(-X, -Y, -Z, 0);
                return new Vector3(ans.x, ans.y, ans.z);
            }

            public override string ToString()
            {
                return $"{XSquared}x^2 + {YSquared}y^2 + {ZSquared}z^2 + {XY}xy + {XZ}xz + {YZ}yz + {X}x + {Y}y + {Z}z";
            }
        }

        private static Vector3 ClampVector(Vector3 value, Vector3 lower, Vector3 upper)
        {
            return new Vector3
            {
                x = Mathf.Clamp(value.x, lower.x, upper.x),
                y = Mathf.Clamp(value.y, lower.y, upper.y),
                z = Mathf.Clamp(value.z, lower.z, upper.z)
            };
        }

        protected override Vector3 CalculateVertex(PointCloud nodes, ImplicitSurface surface, Vector3Int index)
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

            Unbounded3DPolynomial polynomial = new Unbounded3DPolynomial();
            //List<Tuple<Vector3, Vector3>> pointsAndNormals = new List<Tuple<Vector3, Vector3>>();
            foreach ((Vector3Int a, Vector3Int b) in edges)
            {
                Sample nodeA = nodes[index + a];
                Sample nodeB = nodes[index + b];

                if (nodeA.SignBit == nodeB.SignBit)
                {
                    continue;
                }

                float valA = nodeA.Val;
                float valB = nodeB.Val;
                float dist = valB / (valB - valA);
                Vector3 pos = dist * nodeA.Pos + (1 - dist) * nodeB.Pos;

                Vector3 normal = surface.Gradient.EvaluateOnce(pos);

                //pointsAndNormals.Add(new Tuple<Vector3, Vector3>(pos, normal.normalized));

                polynomial.AddPlanarWeighting(pos, normal);
            }

            Vector3 point = polynomial.FindTurningPoint();

            // Clamp
            Vector3 bottomLeft = nodes[index].Pos;
            Vector3 topRight = nodes[index + Vector3Int.one].Pos;
            //point = ClampVector(point, bottomLeft, topRight);

            return point;
        }
    }
}
