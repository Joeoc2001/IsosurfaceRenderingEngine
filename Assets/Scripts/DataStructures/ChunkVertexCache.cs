using System;
using System.Collections;
using System.Collections.Generic;

public class ChunkVertexCache
{
    public readonly int Width;
    private readonly int[,,,] Values;
    public ChunkVertexCache(int width)
    {
        Width = width;
        Values = new int[width, width, width, 3];
    }
    public void Clear()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                for (int k = 0; k < Width; k++)
                {
                    Values[i, j, k, 0] = 0;
                    Values[i, j, k, 1] = 0;
                    Values[i, j, k, 2] = 0;
                }
            }
        }
    }
    public bool IsSet(int x, int y, int z, int axis)
    {
        return Values[x, y, z, axis] != 0;
    }
    public void Set(int x, int y, int z, int axis, int value)
    {
        if (value == -1)
        {
            throw new ArgumentOutOfRangeException("Value can't be -1");
        }

        Values[x, y, z, axis] = value + 1;
    }
    public int Get(int x, int y, int z, int axis)
    {
        return Values[x, y, z, axis] - 1;
    }
}
