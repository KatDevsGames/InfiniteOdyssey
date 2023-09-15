using System.Collections;
using System.Collections.Generic;

namespace InfiniteOdyssey.Extensions;

public class RewindableQueue<T> : IEnumerable<T>, IRewindable
{
    private readonly LinkedList<T> m_list;

    public int Count { get; private set; }

    private LinkedListNode<T>? m_first;
    private LinkedListNode<T>? m_last;

    private struct QueueState
    {
        public int count;
        public LinkedListNode<T> first;
        public LinkedListNode<T> last;
    }

    private readonly Stack<QueueState> m_states = new();

    public int Version => m_states.Count;
    public void RewindTo(int version)
    {
        if (version == 0)
        {
            Count = 0;
            m_first = null;
            m_last = null;
            m_list.Clear();
            return;
        }

        if (version > m_states.Count) throw new IRewindable.RewindException();

        while (m_states.Count > version) m_states.Pop();
        QueueState state = m_states.Peek();
        Count = state.count;
        m_first = state.first;
        m_last = state.last;
        while (m_list.Last != m_last) m_list.RemoveLast();
    }

    private void Update() => m_states.Push(new QueueState
    {
        count = Count,
        first = m_first,
        last = m_last
    });

    public RewindableQueue() => m_list = new LinkedList<T>();

    public RewindableQueue(IEnumerable<T> collection) => m_list = new LinkedList<T>(collection);

    public IEnumerator<T> GetEnumerator()
    {
        if (m_list.Count == 0) yield break;
        var first = m_first;
        var last = m_last;
        var node = first;
        do
        {
            yield return node.Value;
            if (node == last) break;
            node = node.Next;
        } while (node != null);
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Enqueue(T item)
    {
        try
        {
            m_list.AddLast(item);
            Count++;
            m_last = m_list.Last;
            if (Count == 1) m_first = m_last;

        }
        finally { Update(); }
    }

    public bool TryDequeue(out T item)
    {
        try
        {
            if (Count == 0)
            {
                item = default;
                return false;
            }

            Count--;
            item = m_first.Value;
            m_first = m_first.Next;
            return true;
        }
        finally { Update(); }
    }

    public void Clear()
    {
        m_list.Clear();
        m_states.Clear();
        Count = 0;
    }
}