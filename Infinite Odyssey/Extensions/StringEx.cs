using System;

namespace InfiniteOdyssey.Extensions;

public static class StringEx
{
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        if (value.Length == 1) return char.ToLowerInvariant(value[0]).ToString();
        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    public static string ToCamelCase(this Enum value) => value.ToString("G").ToCamelCase();

    public static unsafe string[] Chop(this string value, int chopLength)
    {
        int len = value.Length;
        char* segment = stackalloc char[chopLength];
        string[] result = new string[len];
        for (int i = 0; i < len; i += chopLength)
        {
            int j = 0;
            for (; j < chopLength; j++)
            {
                int next = i + j;
                if (next >= len) break;
                segment[j] = value[next];
            }
            result[i / chopLength] = new string(segment, 0, j);
        }
        return result;
    }
}