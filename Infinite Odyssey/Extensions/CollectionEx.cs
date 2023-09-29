using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace InfiniteOdyssey.Extensions;

public static class CollectionEx
{
    public static void Fill<T>(this IList<T> collection, T item)
    {
        for (int i = 0; i < collection.Count; i++)
            collection[i] = item;
    }

    [ConsumesRNG]
    public static List<T> Shuffle<T>(this List<T> list, RNG _rng)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.IRandom(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    [ConsumesRNG]
    public static Queue<T> Shuffle<T>(this Queue<T> queue, RNG _rng)
    {
        List<T> items = new(queue);
        items.Shuffle(_rng);
        queue.Clear();
        queue.TryAddRange(items);
        return queue;
    }

    public static bool TryPopFirst<T>(this LinkedList<T> list, [MaybeNullWhen(false)] out T value)
    {
        LinkedListNode<T>? node = list.First;
        if (node == null)
        {
            value = default;
            return false;
        }
        value = node.Value;
        list.RemoveFirst();
        return true;
    }

    public static bool TryPopLast<T>(this LinkedList<T> list, [MaybeNullWhen(false)] out T value)
    {
        LinkedListNode<T>? node = list.Last;
        if (node == null)
        {
            value = default;
            return false;
        }
        value = node.Value;
        list.RemoveLast();
        return true;
    }

    public static T PopFirst<T>(this LinkedList<T> list)
    {
        T result = list.First.Value;
        list.RemoveLast();
        return result;
    }

    public static T PopLast<T>(this LinkedList<T> list)
    {
        T result = list.Last.Value;
        list.RemoveLast();
        return result;
    }

    public static bool TryRemove<T>(this IEnumerable<T> collection, T value)
    {
        switch (collection)
        {
            /*case Queue<T> qu:
                {
                    bool result = qu.Count > 0;
                    if (result) value = result ? qu.Dequeue() : default;
                    else value = default;
                    return result;
                }
            case RewindableQueue<T> qu:
                {
                    return qu.TryDequeue(out value);
                }*/
            case Stack<T> st:
                {
                    // this is slightly cursed - replace if problematic
                    T obj = st.Pop();
                    if (obj?.Equals(value) ?? (value == null)) return true;
                    bool result = st.TryRemove(value);
                    st.Push(obj);
                    return result;
                }
            case RandomList<T> rl:
                {
                    return rl.Remove(value);
                }
            case List<T> li:
                {
                    return li.Remove(value);
                }
            case LinkedList<T> ll:
                {
                    return ll.Remove(value);
                }
            case HashSet<T> hs:
                {
                    return hs.Remove(value);
                }
            case RewindableHashSet<T> hs:
                {
                    return hs.Remove(value);
                }
            default:
                return false;
        }
    }

    public static int IndexOf<T>(this IList<T> list, T value)
    {
        int lc = list.Count;
        for (int i = 0; i < lc; i++)
        {
            if (Equals(list[i], value)) return i;
        }
        return -1;
    }

    public static bool TryRemove<T>(this IEnumerable<T> collection, out T value)
    {
        switch (collection)
        {
            case Queue<T> qu:
                {
                    bool result = qu.Count > 0;
                    if (result) value = result ? qu.Dequeue() : default;
                    else value = default;
                    return result;
                }
            case RewindableQueue<T> qu:
                {
                    return qu.TryDequeue(out value);
                }
            case Stack<T> st:
                {
                    bool result = st.Count > 0;
                    if (result) value = result ? st.Pop() : default;
                    else value = default;
                    return result;
                }
            case RandomList<T> rl:
                {
                    bool result = rl.Count > 0;
                    if (result) value = result ? rl.RemoveRandom() : default;
                    else value = default;
                    return result;
                }
            case List<T> li:
                {
                    bool result = li.Count > 0;
                    if (result)
                    {
                        int i = li.Count - 1;
                        value = li[i];
                        li.RemoveAt(i);
                    }
                    else value = default;
                    return result;
                }
            case LinkedList<T> ll:
                {
                    bool result = ll.Count > 0;
                    if (result)
                    {
                        value = ll.Last.Value;
                        ll.RemoveLast();
                    }
                    else value = default;
                    return result;
                }
            case HashSet<T> hs:
                {
                    bool result = hs.Count > 0;
                    if (result)
                    {
                        value = hs.First();
                        hs.Remove(value);
                    }
                    else value = default;
                    return result;
                }
            case RewindableHashSet<T> hs:
                {
                    bool result = hs.Count > 0;
                    if (result)
                    {
                        value = hs.First();
                        hs.Remove(value);
                    }
                    else value = default;
                    return result;
                }
            default:
                value = default;
                return false;
        }
    }

    public static bool TryAdd<T>(this IEnumerable<T> collection, T value)
    {
        switch (collection)
        {
            case Queue<T> qu:
                {
                    qu.Enqueue(value);
                    return true;
                }
            case RewindableQueue<T> qu:
                {
                    qu.Enqueue(value);
                    return true;
                }
            case Stack<T> st:
                {
                    st.Push(value);
                    return true;
                }
            case List<T> rl:
                {
                    rl.Add(value);
                    return true;
                }
            case LinkedList<T> rl:
                {
                    rl.AddLast(value);
                    return true;
                }
            case HashSet<T> hs:
                {
                    hs.Add(value);
                    return true;
                }
            case RewindableHashSet<T> hs:
                {
                    hs.Add(value);
                    return true;
                }
            case ICollection<T> ic:
                {
                    ic.Add(value);
                    return true;
                }
            default:
                return false;
        }
    }

    public static bool TryAddRange<T>(this IEnumerable<T> collection, IEnumerable<T> values)
    {
        switch (collection)
        {
            case Queue<T> qu:
                {
                    foreach (T value in values) { qu.Enqueue(value); }
                    return true;
                }
            case RewindableQueue<T> qu:
                {
                    foreach (T value in values) { qu.Enqueue(value); }
                    return true;
                }
            case Stack<T> st:
                {
                    foreach (T value in values) { st.Push(value); }
                    return true;
                }
            case List<T> rl:
                {
                    rl.AddRange(values);
                    return true;
                }
            case LinkedList<T> rl:
                {
                    foreach (T value in values) { rl.AddLast(value); }
                    return true;
                }
            case HashSet<T> hs:
                {
                    foreach (T value in values) { hs.Add(value); }
                    return true;
                }
            case RewindableHashSet<T> hs:
                {
                    foreach (T value in values) { hs.Add(value); }
                    return true;
                }
            case ICollection<T> ic:
                {
                    foreach (T value in values) { ic.Add(value); }
                    return true;
                }
            default:
                return false;
        }
    }

    public static bool TryAddRange<K, V>(this IDictionary<K, V> collection, IEnumerable<KeyValuePair<K, V>> values)
    {
        switch (collection)
        {
            case { } d:
                {
                    foreach (var v in values) d.Add(v.Key, v.Value);
                    return true;
                }
            default:
                return false;
        }
    }

    public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> collection) where K : notnull
    {
        Dictionary<K, V> result = new();
        foreach (var next in collection)
        {
            result.Add(next.Key, next.Value);
        }
        return result;
    }

    public static Dictionary<K, V> ToDictionary<T, K, V>(this IEnumerable<T> collection, Func<T, KeyValuePair<K, V>> selector) where K : notnull
    {
        Dictionary<K, V> result = new();
        foreach (T next in collection)
        {
            var s = selector(next);
            result.Add(s.Key, s.Value);
        }
        return result;
    }

    public static bool TryGetKey<K, V>(this Dictionary<K, V> dictionary, V value, [MaybeNullWhen(false)] out K key) where K : notnull
        => TryGetKey((IReadOnlyCollection<KeyValuePair<K, V>>)dictionary, value, out key);

    public static bool TryGetKey<K, V>(this ICollection<KeyValuePair<K, V>> dictionary, V value, [MaybeNullWhen(false)] out K key) where K : notnull
    {
        foreach (KeyValuePair<K, V> kvp in dictionary)
        {
            if (!Equals(value, kvp.Value)) continue;
            key = kvp.Key;
            return true;
        }
        key = default;
        return false;
    }

    public static bool TryGetKey<K, V>(this IReadOnlyCollection<KeyValuePair<K, V>> dictionary, V value, [MaybeNullWhen(false)] out K key) where K : notnull
    {
        foreach (KeyValuePair<K, V> kvp in dictionary)
        {
            if (!Equals(value, kvp.Value)) continue;
            key = kvp.Key;
            return true;
        }
        key = default;
        return false;
    }

    public static bool TryIncrement<K>(this IDictionary<K, int> dictionary, K key)
        => TryDecrement(dictionary, key, int.MaxValue, out _);

    public static bool TryIncrement<K>(this IDictionary<K, int> dictionary, K key, int maximum)
        => TryDecrement(dictionary, key, maximum, out _);

    public static bool TryIncrement<K>(this IDictionary<K, int> dictionary, K key, out int newValue)
        => TryDecrement(dictionary, key, int.MaxValue, out newValue);

    public static bool TryIncrement<K>(this IDictionary<K, int> dictionary, K key, int maximum, out int newValue)
    {
        if (!dictionary.TryGetValue(key, out newValue)) newValue = 0;
        if (newValue >= maximum) return false;
        dictionary[key] = ++newValue;
        return true;
    }

    public static bool TryDecrement<K>(this IDictionary<K, int> dictionary, K key)
        => TryDecrement(dictionary, key, 0, out _);

    public static bool TryDecrement<K>(this IDictionary<K, int> dictionary, K key, int minimum)
        => TryDecrement(dictionary, key, minimum, out _);

    public static bool TryDecrement<K>(this IDictionary<K, int> dictionary, K key, out int newValue)
        => TryDecrement(dictionary, key, 0, out newValue);

    public static bool TryDecrement<K>(this IDictionary<K, int> dictionary, K key, int minimum, out int newValue)
    {
        if (!dictionary.TryGetValue(key, out newValue)) newValue = 0;
        if (newValue <= minimum) return false;
        dictionary[key] = --newValue;
        return true;
    }

    public static T TakeRandom<T>(this IEnumerable<T> collection, RNG rng)
    {
        int max = collection.Count() - 1;
        return collection.Skip(rng.IRandom(0, max)).First();
    }

    public static int Find<T>(this T[] list, Predicate<T> predicate)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (predicate(list[i])) { return i; }
        }
        return -1;
    }

    public static List<T> Resize<T>(this List<T> list, int newSize) => Resize(list, newSize, () => default(T));

    public static List<T> Resize<T>(this List<T> list, int newSize, T value)
        => Resize(list, newSize, () => value);

    public static List<T> Resize<T>(this List<T> list, int newSize, Func<T> valueSource)
    {
        int cur = list.Count;
        if (newSize < cur) list.RemoveRange(newSize, cur - newSize);
        else if (newSize > cur)
        {
            if (newSize > list.Capacity) { list.Capacity = newSize; }
            list.AddRange(Enumerable.Repeat(valueSource(), newSize - cur));
        }
        return list;
    }

    public static List<List<T>> Resize<T>(this List<List<T>> list, int newX, int newY)
        => Resize(list, newX, newY, () => default(T));

    public static List<List<T>> Resize<T>(this List<List<T>> list, int newX, int newY, T value)
        => Resize(list, newX, newY, () => value);

    public static List<List<T>> Resize<T>(this List<List<T>> list, int newX, int newY, Func<T> valueSource)
    {
        list.Resize(newX, () => new());
        foreach (List<T> subList in list) subList.Resize(newY, valueSource());
        return list;
    }

    public static V Get<K, V>(this Dictionary<K, V> dictionary, K key) where V : new() where K : notnull
    {
        if (dictionary.ContainsKey(key)) { return dictionary[key]; }
        V value = new();
        dictionary.Add(key, value);
        return value;
    }

    public static IEnumerable<T> WithoutAny<T>(this IEnumerable<T> source, T value)
        => Without(source, value, int.MaxValue);

    public static IEnumerable<T> WithoutOne<T>(this IEnumerable<T> source, T value)
        => Without(source, value, 1);

    public static IEnumerable<T> Without<T>(this IEnumerable<T> source, T value, int count)
    {
        int remaining = count;
        foreach (T next in source)
        {
            if ((remaining > 0) && (EqualityComparer<T>.Default.Equals(next, value)))
            {
                remaining -= 1;
                continue;
            }
            yield return next;
        }
    }

    [ConsumesRNG]
    public static IList<T> Shuffle<T>(this IList<T> list, RNG _rng)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _rng.IRandom(0, n);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    public static bool Remove<T>(this List<T> list, IEnumerable<T> values)
    {
        bool result = false;
        foreach (T value in values)
        {
            result = list.Remove(value) || result;
        }
        return result;
    }

    public static IEnumerable<T> TakeIndexes<T>(this IEnumerable<T> list, IEnumerable<int> indexes)
    {
        int i = 0;
        foreach (T item in list)
        {
            if (indexes.Contains(i)) yield return item;
            i++;
        }
    }

    public static IEnumerable<T> TakeIndexes<T>(this IEnumerable<T> list, int indexes)
    {
        int i = 0;
        foreach (T item in list)
        {
            int v = 1 << i;
            if ((indexes & v) == v) yield return item;
            i++;
        }
    }

    public static IEnumerable<int> FindMany<T>(this IEnumerable<T> list, Predicate<T> predicate)
    {
        int i = 0;
        foreach (T item in list)
        {
            if (predicate(item)) yield return i;
            i++;
        }
    }

    public static int FindManyBits<T>(this IEnumerable<T> list, Predicate<T> predicate)
    {
        int result = 0;
        int i = 0;
        foreach (T item in list)
        {
            if (predicate(item)) result += (1 << i);
            i++;
        }
        return result;
    }

    public static int Find<T>(this IList<T> list, Predicate<T> predicate)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (predicate(list[i])) { return i; }
        }
        return -1;
    }

    public static int Find<T>(this IEnumerable<T> list, Predicate<T> predicate)
    {
        int i = 0;
        foreach (T item in list)
        {
            if (predicate(item)) { return i; }
            i++;
        }
        return -1;
    }
}