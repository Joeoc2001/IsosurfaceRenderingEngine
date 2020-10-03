using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkVertexCache
{
    public readonly Vector3Int Size;
    public readonly int Depth;
    private readonly int[,,,] _values;
    public ChunkVertexCache(Vector3Int width, int depth)
    {
        Size = width;
        Depth = depth;
        _values = new int[width.x, width.y, width.z, depth];
    }
    public void Clear()
    {
        Array.Clear(_values, 0, Size.x * Size.y * Size.z * Depth);
    }
    public bool IsSet(Vector3Int p, int d)
    {
        return IsSet(p.x, p.y, p.z, d);
    }
    public bool IsSet(int x, int y, int z, int d)
    {
        return _values[x, y, z, d] != 0;
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

        _values[x, y, z, d] = value + 1;
    }
    public int Get(Vector3Int p, int d)
    {
        return Get(p.x, p.y, p.z, d);
    }
    public int Get(int x, int y, int z, int d)
    {
        return _values[x, y, z, d] - 1;
    }
}
