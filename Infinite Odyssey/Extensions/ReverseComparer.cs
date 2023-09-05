using System;
using System.Collections.Generic;

namespace InfiniteOdyssey.Extensions;

public class ReverseComparer<T> : IComparer<T> where T : struct, IComparable<T>
{
    public static readonly ReverseComparer<T> Instance = new();

    public int Compare(T x, T y) => y.CompareTo(x);
}