using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkVertexCache
{
    public readonly int Width;
    public readonly int Depth;
    private readonly int[,,,] Values;
    public ChunkVertexCache(int width, int depth)
    {
        Width = width;
        Depth = depth;
        Values = new int[width, width, width, depth];
    }
    public void Clear()
    {
        Array.Clear(Values, 0, Width * Width * Width * Depth);
    }
    public bool IsSet(Vector3Int p, int d)
    {
        return IsSet(p.x, p.y, p.z, d);
    }
    public bool IsSet(int x, int y, int z, int d)
    {
        return Values[x, y, z, d] != 0;
    }
    public void Set(Vector3Int p, int d, int value)
    {
        Set(p.x, p.y, p.z, d, value);
    }
    public void Set(int x, int y, int z, int d, int value)
    {
        if (value == -1)
        {
            throw new ArgumentOutOfRangeException("Value can't be -1");
        }

        Values[x, y, z, d] = value + 1;
    }
    public int Get(Vector3Int p, int d)
    {
        return Get(p.x, p.y, p.z, d);
    }
    public int Get(int x, int y, int z, int d)
    {
        return Values[x, y, z, d] - 1;
    }
}
