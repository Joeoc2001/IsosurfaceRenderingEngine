using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSet : MonoBehaviour
{
    public Chunk baseChunk;

    [Range(1, 50)]
    public int viewDistance;

    [Range(0, 50)]
    public float normalApproxDistance;

    [Range(0, 2)]
    public float qualityDropoff;

    private Dictionary<Int3, Chunk> chunks;

    // Start is called before the first frame update
    void Start()
    {
        chunks = new Dictionary<Int3, Chunk>();
    }

    bool ShouldApproximateNormals(double dist)
    {
        return dist >= normalApproxDistance;
    }

    int GetQuality(double dist)
    {
        return (int)Math.Max(1,
                baseChunk.quality - (qualityDropoff * dist) + 0.5
            );
    }

    // Update is called once per frame
    void Update()
    {
        Int3 baseIndex = new Int3(transform.position / baseChunk.size);

        for (int x = -viewDistance; x < viewDistance; x++)
        {
            for (int y = -viewDistance; y < viewDistance; y++)
            {
                for (int z = -viewDistance; z < viewDistance; z++)
                {
                    Int3 index = baseIndex + new Int3(x, y, z);

                    double dist = Math.Sqrt(x * x + y * y + z * z);

                    Chunk chunk;
                    if (!chunks.TryGetValue(index, out chunk))
                    {
                        Vector3 offset = index * baseChunk.size;
                        chunk = Instantiate<Chunk>(baseChunk, offset, Quaternion.identity, transform);

                        chunks.Add(index, chunk);
                    }

                    chunk.approximateNormals = baseChunk.approximateNormals || ShouldApproximateNormals(dist);
                    chunk.quality = GetQuality(dist);

                    chunk.GenerateMesh();
                }
            }
        }
        Debug.Log("Delta time: " + Time.deltaTime);
    }
}