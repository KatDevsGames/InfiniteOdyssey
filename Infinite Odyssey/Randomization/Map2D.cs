using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Range = InfiniteOdyssey.Extensions.Range;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class Map2D<T>
{
    [JsonProperty(PropertyName = "data")]
    private readonly Map1D<Map1D<T?>> m_map = new();

    public Range Range => m_map.Range;

    private Point? m_size;
    public Point Size
    {
        get
        {
            if (m_size.HasValue) return m_size.Value;
            int minOffset = Int32.MaxValue;
            int maxSize = Int32.MinValue;
            foreach (Map1D<T?> col in m_map)
            {
                minOffset = Math.Min(minOffset, col.Offset);
                maxSize = Math.Max(maxSize, col.Range.Maximum + 1);
            }
            return (Point)(m_size = new Point(m_map.Range.Maximum + 1, (minOffset == Int32.MaxValue) ? 0 : (maxSize - minOffset)));
        }
    }

    public Map2D() => m_map.SizeChanged += OnSizeChanged;

    private void OnSizeChanged(object? _, EventArgs e) => m_size = null;

    public Map1D<T?> this[int index]
    {
        get
        {
            var col = m_map[index];
            if (col != null) return col;

            col = new Map1D<T?>();
            col.SizeChanged += OnSizeChanged;
            return m_map[index] = col;
        }
    }

    public T? this[Point location]
    {
        get => this[location.X][location.Y];
        set => this[location.X][location.Y] = value;
    }

    public T? this[int x, int y]
    {
        get => this[x][y];
        set => this[x][y] = value;
    }

    public bool IsEmpty(Rectangle rect)
    {
        int xz = rect.X + rect.Width;
        int yz = rect.Y + rect.Height;

        for (int x = rect.X; x < xz; x++)
        for (int y = rect.Y; y < yz; y++)
            if (this[x, y] != null) return false;

        return true;
    }

    public IEnumerable<T> Values
    {
        get
        {
            foreach (Map1D<T?> col in m_map)
            foreach (T? value in col)
                if (value != null)
                    yield return value;
        }
    }

    private IEnumerable<Point> UnmappedPoints()
    {
        foreach (int x in m_map.Range)
        {
            Map1D<T?> col = this[x];
            foreach (int y in col.Range)
            {
                if (col[y] == null) yield return new(x, y);
            }
        }
    }

    public void Trim()
    {
        foreach (int index in m_map.Range)
        {
            Map1D<T?>? col = m_map[index];
            col?.Trim();
            if ((col?.Range ?? Range.Invalid) == Range.Invalid) m_map[index] = null;
        }
        m_map.Trim();
    }
}