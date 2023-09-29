using System.Collections.Generic;

namespace InfiniteOdyssey.Extensions;

public static class StructEx
{
    public static T IfDefault<T>(this T value, T replacement) where T : struct
        => EqualityComparer<T>.Default.Equals(value, default) ? replacement : value;

    public static T IfValue<T>(this T value, T comparand, T replacement) where T : struct
        => EqualityComparer<T>.Default.Equals(value, comparand) ? replacement : value;
}