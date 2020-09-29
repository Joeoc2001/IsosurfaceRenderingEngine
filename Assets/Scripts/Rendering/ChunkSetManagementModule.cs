using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDFRendering
{
    public abstract class ChunkSetManagementModule : MonoBehaviour
    {
        public virtual void Init(ChunkSet set, ChunkSystem system)
        {

        }

        /// <summary>
        /// Runs the module once
        /// </summary>
        /// <param name="set">The set to run the module on</param>
        /// <param name="system">The system of rules the module should be run with</param>
        public virtual void Tick(ChunkSet set, ChunkSystem system)
        {

        }

        public virtual void Destruct(ChunkSet set, ChunkSystem system)
        {

        }
    }
}