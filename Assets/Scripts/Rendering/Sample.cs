using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDFRendering
{
    public struct Sample
    {
        private readonly PointCloud _cloud;
        private readonly Vector3Int _index;

        public Sample(PointCloud cloud, Vector3Int index)
        {
            _cloud = cloud ?? throw new ArgumentNullException(nameof(cloud));
            _index = index;
        }

        public int SignBit { get => _cloud.GetSignBit(_index); }
        public Vector3 Pos { get => _cloud.GetPosition(_index); }
        public float Val { get => _cloud.GetValue(_index); }
    }
}