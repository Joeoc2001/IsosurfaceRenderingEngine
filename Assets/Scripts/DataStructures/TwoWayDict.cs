using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoWayDict<T, U>
{
    private readonly Dictionary<T, U> dict1;
    private readonly Dictionary<U, T> dict2;

    public TwoWayDict()
    {
        this.dict1 = new Dictionary<T, U>();
        this.dict2 = new Dictionary<U, T>();
    }

    public U this[T key]
    {
        get
        {
            return dict1[key];
        }
    }

    public T this[U key]
    {
        get
        {
            return dict2[key];
        }
    }

    public int Count
    {
        get
        {
            return dict1.Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return false;
        }
    }

    public void Add(T key, U value)
    {
        dict1.Add(key, value);
        dict2.Add(value, key);
    }

    public void Add(U key, T value)
    {
        Add(value, key);
    }

    public void Clear()
    {
        dict1.Clear();
        dict2.Clear();
    }

    public bool ContainsKey(T key)
    {
        return dict1.ContainsKey(key);
    }

    public bool ContainsKey(U key)
    {
        return dict2.ContainsKey(key);
    }

    public bool Remove(T key)
    {
        if (!dict1.ContainsKey(key))
        {
            return false;
        }
        dict2.Remove(dict1[key]);
        return dict1.Remove(key);
    }

    public bool Remove(U key)
    {
        if (!dict2.ContainsKey(key))
        {
            return false;
        }
        dict1.Remove(dict2[key]);
        return dict2.Remove(key);
    }

    public bool TryGetValue(T key, out U value)
    {
        return dict1.TryGetValue(key, out value);
    }

    public bool TryGetValue(U key, out T value)
    {
        return dict2.TryGetValue(key, out value);
    }
}
