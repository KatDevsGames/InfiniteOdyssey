using System;
using System.Collections;
using System.Collections.Generic;

namespace InfiniteOdyssey.Extensions;

public class RewindableHashSet<T> : ICollection<T>, IRewindable
{
    private readonly HashSet<T> m_set = new();

    private readonly LinkedList<IDictEvent> m_events = new();

    private interface IDictEvent { }

    private struct AddEvent : IDictEvent
    {
        public readonly T key;
        public readonly bool result;

        public AddEvent(T key, bool result)
        {
            this.key = key;
            this.result = result;
        }
    }

    private struct RemoveEvent : IDictEvent
    {
        public readonly T key;
        public readonly bool result;

        public RemoveEvent(T key, bool result)
        {
            this.key = key;
            this.result = result;
        }
    }

    private struct ClearEvent : IDictEvent { }

    public int Version => m_events.Count;
    public void RewindTo(int version)
    {
        if (version == 0)
        {
            m_set.Clear();
            m_events.Clear();
        }
        if (version > m_events.Count) throw new IRewindable.RewindException();
        while (m_events.Count > version)
        {
            switch (m_events.PopLast())
            {
                case AddEvent de:
                    if (de.result) m_set.Remove(de.key);
                    break;
                case RemoveEvent de:
                    if (de.result) m_set.Add(de.key);
                    break;
                case ClearEvent:
                    m_set.Clear();
                    foreach (IDictEvent? ev in m_events)
                    {
                        switch (ev)
                        {
                            case AddEvent lev:
                                m_set.Add(lev.key);
                                break;
                            case RemoveEvent lev:
                                m_set.Remove(lev.key);
                                break;
                            case ClearEvent:
                                m_set.Clear();
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public IEnumerator<T> GetEnumerator() => m_set.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void ICollection<T>.Add(T key) => Add(key);
    public bool Add(T key)
    {
        bool result = m_set.Add(key);
        m_events.AddLast(new AddEvent(key, result));
        return result;
    }

    public void Clear()
    {
        m_set.Clear();
        m_events.AddLast(new ClearEvent());
    }

    public bool Contains(T key) => m_set.Contains(key);

    public void CopyTo(T[] array, int arrayIndex) => m_set.CopyTo(array, arrayIndex);

    public bool Remove(T key)
    {
        bool result = m_set.Remove(key);
        m_events.AddLast(new RemoveEvent(key, result));
        return result;
    }

    public int Count => m_set.Count;
    bool ICollection<T>.IsReadOnly => false;
}