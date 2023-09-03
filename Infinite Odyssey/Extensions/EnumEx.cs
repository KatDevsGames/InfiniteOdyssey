using System;
using System.Collections.Generic;

namespace InfiniteOdyssey.Extensions;

internal static class EnumEx
{
    public static IEnumerable<T> GetFlags<T>(this T value, bool includeZero = false) where T : Enum
    {
        foreach (T val in Enum.GetValues(typeof(T)))
        {
            ulong lv = Convert.ToUInt64(val);
            if ((!includeZero) && (lv == 0)) continue;
            if ((lv & Convert.ToUInt64(value)) == lv) yield return val;
        }
    }
}