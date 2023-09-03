using System.Collections;
using System.Collections.Generic;

namespace InfiniteOdyssey.Extensions;

public class TransactionList<T> : IList<T>
{
    private readonly List<T> m_list;
    private readonly List<T> m_temp;

    public TransactionList()
    {
        m_list = new List<T>();
        m_temp = new List<T>();
    }

    public TransactionList(IEnumerable<T> collection)
    {
        m_list = new List<T>(collection);
        m_temp = new List<T>(m_list);
    }

    public TransactionList(int capacity)
    {
        m_list = new List<T>(capacity);
        m_temp = new List<T>(capacity);
    }

    public IEnumerator<T> GetEnumerator() => m_list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => m_list.GetEnumerator();


    public void Add(T item) => m_temp.Add(item);

    public void Clear() => m_temp.Clear();

    public bool Contains(T item) => m_temp.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => m_temp.CopyTo(array, arrayIndex);

    public bool Remove(T item) => m_temp.Remove(item);

    public int Count => m_temp.Count;
    public bool IsReadOnly => false;
    public int IndexOf(T item) => m_temp.IndexOf(item);

    public void Insert(int index, T item) => m_temp.Insert(index, item);

    public void RemoveAt(int index) => m_temp.RemoveAt(index);

    public T this[int index]
    {
        get => m_temp[index];
        set => m_temp[index] = value;
    }

    public void Commit()
    {
        m_list.Clear();
        m_list.AddRange(m_temp);
    }

    public void Rollback()
    {
        m_temp.Clear();
        m_temp.AddRange(m_list);
    }
}