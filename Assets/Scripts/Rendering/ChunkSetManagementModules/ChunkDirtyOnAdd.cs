using UnityEngine;

namespace SDFRendering.ChunkSetManagementModules
{
    public class ChunkDirtyOnAdd : ChunkSetManagementModule
    {
        public override void Init(ChunkSet set, ChunkSystem system)
        {
            set.OnChunkAdded += (s, c, i) => Set_OnChunkAdded(c);
        }

        private void Set_OnChunkAdded(Chunk chunk)
        {
            chunk.Dirty = true;
        }
    }
}
