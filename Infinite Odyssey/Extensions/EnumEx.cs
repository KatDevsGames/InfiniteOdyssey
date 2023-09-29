using System;
using System.Collections.Generic;

namespace InfiniteOdyssey.Extensions;

public static class EnumEx
{
    public static IEnumerable<T> GetFlags<T>(this T value, bool includeZero = false) where T : struct, Enum
    {
        foreach (T val in Enum.GetValues(typeof(T)))
        {
            ulong lv = Convert.ToUInt64(val);
            if ((!includeZero) && (lv == 0)) continue;
            if ((lv & Convert.ToUInt64(value)) == lv) yield return val;
        }
    }

    public static unsafe T TakeRandomValue<T>(this T value, RNG rng) where T : struct, Enum
    {
        T[] values = EnumEx<T>.Values;
        int* matchIndexes = stackalloc int[values.Length];
        int m = 0;
        for (int i = 0; i < values.Length; i++)
        {
            T v = values[i];
            if (!value.HasFlag(v)) continue;
            matchIndexes[m++] = i;
        }

        return (m == 0) ? default : values[matchIndexes[rng.IRandom(m - 1)]];
    }

    public static unsafe string TakeRandomName<T>(this T value, RNG rng) where T : struct, Enum
    {
        T[] values = EnumEx<T>.Values;
        int* matchIndexes = stackalloc int[values.Length];
        int m = 0;
        for (int i = 0; i < values.Length; i++)
        {
            T v = values[i];
            if (!value.HasFlag(v)) continue;
            matchIndexes[m++] = i;
        }

        return (m == 0) ? default : EnumEx<T>.Names[matchIndexes[rng.IRandom(m - 1)]];
    }
}

public static class EnumEx<T> where T : struct, Enum
{
    public static readonly string[] Names = Enum.GetNames<T>();
    public static readonly T[] Values = Enum.GetValues<T>();

    public static T TakeRandomValue(RNG rng)
        => Values.TakeRandom(rng);
}