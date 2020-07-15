using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

/// <summary>
/// Implements a queue with the ability to remove any item in the queue in O(1) amortized time.
/// Does not support duplicate items.
/// For a simple proof, let cost = (head - tail - items) % length of backing array
/// and assume O(1) array doubling amortized cost.
/// </summary>
/// <typeparam name="T">The type of the data stored in this queue</typeparam>
public class FastRemovableQueue<T>
{
    // Essentially a two-way dict but slightly faster
    // Keep a log of where items are if they need to be removed so that all operations are O(1)
    private T[] backingData = new T[1];
    private readonly Dictionary<T, int> indexes = new Dictionary<T, int>(); 

    private int head = 0;
    private int tail = 0;

    public int Length { get; private set; } = 0;

    /// <summary>
    /// Pushes the item to the back of the queue
    /// </summary>
    /// <exception cref="ArgumentNullException">If the item is null or default</exception>
    /// <exception cref="InvalidOperationException">If the item is already in the queue</exception>
    /// <param name="item">Cannot be null</param>
    public void Push(T item)
    {
        if (item.Equals(default(T)))
        {
            throw new ArgumentNullException("Cannot add a default (null) item to this queue");
        }

        if (indexes.ContainsKey(item))
        {
            throw new InvalidOperationException("Item is already present in the queue");
        }

        // Check if we need to add more space
        if (Length >= backingData.Length)
        {
            Resize(backingData.Length * 2);
        }

        // Check if we need to defrag the array
        if (head == tail && Length != 0)
        {
            Resize(backingData.Length);
        }

        indexes.Add(item, head);
        backingData[head] = item;

        Length++;
        head++;
        head %= backingData.Length;
    }

    /// <summary>
    /// Pops the first item from the queue
    /// </summary>
    /// <returns>The first item</returns>
    public T Pop()
    {
        if (Length == 0)
        {
            throw new InvalidOperationException("Queue is empty");
        }

        T item;

        do
        {
            item = backingData[tail];

            tail++;
            tail %= backingData.Length;

        } while (item.Equals(default(T)));

        Remove(item);

        if (Length <= backingData.Length / 4)
        {
            Resize(backingData.Length / 2);
        }

        return item;
    }

    /// <summary>
    /// Removes the item from the queue, throws InvalidOperationException if item is not present
    /// O(1)
    /// </summary>
    /// <param name="item">The item to remove</param>
    public void Remove(T item)
    {
        if (!TryRemoveItem(item))
        {
            throw new InvalidOperationException("Item not in queue");
        }
    }

    /// <summary>
    /// Removes the item from the queue
    /// </summary>
    /// <param name="item">The item to remove</param>
    /// <returns>True if the item was present, false if no item was present to be removed</returns>
    public bool TryRemoveItem(T item)
    {
        if (!indexes.TryGetValue(item, out int index))
        {
            return false;
        }

        backingData[index] = default;
        indexes.Remove(item);

        Length--;

        return true;
    }

    /// <summary>
    /// Ensures that the queue can hold an amount of items.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if size less than items</exception>
    /// <param name="size">The number of items to hold</param>
    public void Resize(int size)
    {
        if (size < Length)
        {
            throw new ArgumentOutOfRangeException("Size is less than the number of items");
        }

        size = Math.Max(size, 1); // Backing array shouldn't ever be smaller than 1 for mod & scaling reasons

        T[] newBackingData = new T[size];

        for (int i = 0; i < Length; i++)
        {
            T item;

            do
            {
                item = backingData[tail];

                tail++;
                tail %= backingData.Length;

            } while (item.Equals(default(T)));

            newBackingData[i] = item;
            indexes[item] = i;
        }

        backingData = newBackingData;

        tail = 0;
        head = Length;
    }
}