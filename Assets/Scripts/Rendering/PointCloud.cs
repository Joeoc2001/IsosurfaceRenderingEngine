using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDFRendering
{
    /**
     * An immutable set of points, each with a position and sampled value 
     */
    public class PointCloud 
    {
        private readonly float[,,] _nodes;
        public readonly bool IsUniform;

        public Vector3 Origin { get; }

        public Vector3 Spacing { get; }

        public PointCloud(float[,,] nodes, Vector3 origin, Vector3 spacing)
        {
            _nodes = nodes;

            Origin = origin;
            Spacing = spacing;

            IsUniform = GetIsUniform(nodes);
        }

        private static bool GetIsUniform(float[,,] nodes)
        {
            if (nodes.Length == 0)
            {
                return true;
            }

            int s = Math.Sign(nodes[0, 0, 0]);

            foreach (float f in nodes)
            {
                if (Math.Sign(f) != s)
                {
                    return false;
                }
            }
            return true;
        }

        public Sample this[int x, int y, int z]
        {
            get
            {
                return this[new Vector3Int(x, y, z)];
            }
        }

        public Sample this[Vector3Int index]
        {
            get
            {
                return new Sample(this, index);
            }
        }

        public Vector3Int Resolution { get => new Vector3Int(_nodes.GetLength(0), _nodes.GetLength(1), _nodes.GetLength(2)); }

        public float GetValue(Vector3Int index)
        {
            return _nodes[index.x, index.y, index.z];
        }

        public Vector3 GetPosition(Vector3Int index)
        {
            Vector3 offset = new Vector3(index.x, index.y, index.z);
            offset.Scale(Spacing);
            return Origin + offset;
        }

        public int GetSignBit(Vector3Int index)
        {
            float value = GetValue(index);

            if (float.IsNaN(value))
            {
                return 1;
            }

            return (Math.Sign(value) - 1) / -2;
        }
    }
}