using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Extensions;

public class RewindableDictionary<K, V> : IDictionary<K, V>, IRewindable, ICloneable where K : notnull
{
    [JsonProperty(PropertyName = "items")]
    private readonly Dictionary<K, V> m_dict = new();

    [JsonProperty(PropertyName = "events")]
    private readonly LinkedList<IDictEvent> m_events = new();

    object ICloneable.Clone() => Clone();
    public RewindableDictionary<K, V> Clone()
    {
        RewindableDictionary<K, V> clone = new();
        clone.m_dict.TryAddRange(m_dict);
        clone.m_events.TryAddRange(m_events);
        return clone;
    }

    private interface IDictEvent { }

    private struct AddEvent : IDictEvent
    {
        public readonly K key;
        public readonly V newValue;

        public AddEvent(K key, V newValue)
        {
            this.key = key;
            this.newValue = newValue;
        }
    }

    private struct RemoveEvent : IDictEvent
    {
        public readonly K key;
        public readonly V oldValue;

        public RemoveEvent(K key, V oldValue)
        {
            this.key = key;
            this.oldValue = oldValue;
        }
    }

    private struct ModifyEvent : IDictEvent
    {
        public readonly K key;
        public readonly V oldValue;
        public readonly V newValue;

        public ModifyEvent(K key, V oldValue, V newValue)
        {
            this.key = key;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }

    private struct ClearEvent : IDictEvent { }

    public int Version => m_events.Count;
    public void RewindTo(int version)
    {
        if (version == 0)
        {
            m_dict.Clear();
            m_events.Clear();
        }
        if (version > m_events.Count) throw new IRewindable.RewindException();
        while (m_events.Count > version)
        {
            switch (m_events.PopLast())
            {
                case AddEvent de:
                    m_dict.Remove(de.key);
                    break;
                case RemoveEvent de:
                    m_dict.Add(de.key, de.oldValue);
                    break;
                case ModifyEvent de:
                    m_dict[de.key] = de.oldValue;
                    break;
                case ClearEvent:
                    m_dict.Clear();
                    foreach (IDictEvent? ev in m_events)
                    {
                        switch (ev)
                        {
                            case AddEvent lev:
                                m_dict.Add(lev.key, lev.newValue);
                                break;
                            case RemoveEvent lev:
                                m_dict.Remove(lev.key);
                                break;
                            case ModifyEvent lev:
                                m_dict[lev.key] = lev.newValue;
                                break;
                            case ClearEvent:
                                m_dict.Clear();
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

    public ICollection<K> Keys => m_dict.Keys;
    public ICollection<V> Values => m_dict.Values;

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => m_dict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item) => Add(item.Key, item.Value);

    public void Add(K key, V value)
    {
        m_dict.Add(key, value);
        m_events.AddLast(new AddEvent(key, value));
    }

    public void Clear()
    {
        m_dict.Clear();
        m_events.AddLast(new ClearEvent());
    }

    bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
        => ((IDictionary<K, V>)m_dict).Contains(item);

    void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        => ((IDictionary<K, V>)m_dict).CopyTo(array, arrayIndex);

    bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        => Remove(item.Key);
    public int Count => m_dict.Count;
    bool ICollection<KeyValuePair<K, V>>.IsReadOnly => false;

    public bool ContainsKey(K key) => m_dict.ContainsKey(key);

    public bool Remove(K key)
    {
        bool result = m_dict.Remove(key, out V value);
        if (result) m_events.AddLast(new RemoveEvent(key, value));
        return result;
    }

    public bool RemoveRandom(RNG rng, out K key)
    {
        if (m_dict.Count == 0)
        {
            key = default;
            return false;
        }
        key = m_dict.Keys.TakeRandom(rng);
        return Remove(key);
    }

    public bool TryGetValue(K key, out V value)
        => m_dict.TryGetValue(key, out value);

    public V this[K key]
    {
        get => m_dict[key];
        set
        {
            if (m_dict.ContainsKey(key)) m_events.AddLast(new ModifyEvent(key, m_dict[key], value));
            else m_events.AddLast(new AddEvent(key, value));
            m_dict[key] = value;
        }
    }
}