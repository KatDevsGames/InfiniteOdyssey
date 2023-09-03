using System.Collections;
using System.Collections.Generic;

namespace InfiniteOdyssey.Extensions;

public class DummyCollection<T> : ICollection<T>, IRewindable
{
    public static readonly DummyCollection<T> Instance = new();

    public IEnumerator<T> GetEnumerator() { yield break; }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(T item) { }
    public void Clear() { }
    public bool Contains(T item) => false;

    public void CopyTo(T[] array, int arrayIndex) { }

    public bool Remove(T item) => false;

    public int Count => 0;
    public bool IsReadOnly => false;
    public int Version => 0;
    public void RewindTo(int version) { }
}