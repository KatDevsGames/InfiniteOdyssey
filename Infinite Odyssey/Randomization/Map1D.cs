using System;
using System.Collections;
using System.Collections.Generic;
using InfiniteOdyssey.Extensions;
using Newtonsoft.Json;
using Range = InfiniteOdyssey.Extensions.Range;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class Map1D<T> : IEnumerable<T?>
{
    [JsonProperty(PropertyName = "offset", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public int Offset { get; private set; }

    [JsonProperty(PropertyName = "data")]
    private readonly List<T?> m_map = new();

    public event EventHandler? SizeChanged;

    public Range Range
    {
        get
        {
            int limit = ((m_map.Count - Offset) - 1);
            if (limit < Offset) return Range.Invalid;
            return Offset..limit;
        }
    }

    public T? this[int index]
    {
        get
        {
            index -= Offset;
            return (index < m_map.Count) ? m_map[index] : default;
        }
        set
        {
            index -= Offset;
            if (index < 0)
            {
                Offset = index;
                while (index++ > 0) m_map.Insert(0, default);
                SizeChanged?.Invoke(this, EventArgs.Empty);
            }
            else if (index > m_map.Count)
            {
                m_map.Resize(index + 1);
                SizeChanged?.Invoke(this, EventArgs.Empty);
            }
            m_map[index] = value;
        }
    }

    public IEnumerator<T> GetEnumerator() => m_map.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Trim()
    {
        int? first = null;
        int last = 0;
        for (int index = 0; index < m_map.Count; index++)
        {
            if (m_map[index] == null) continue;
            first ??= index;
            last = index;

        }

        if (first == null) return;
        int width = (last - first.Value) + 1;
        m_map.RemoveRange(0, first.Value);
        m_map.Resize(width);
        Offset = first.Value;
        SizeChanged?.Invoke(this, EventArgs.Empty);
    }
}