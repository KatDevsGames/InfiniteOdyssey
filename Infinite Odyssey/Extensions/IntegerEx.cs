using System;

namespace InfiniteOdyssey.Extensions;

public static class IntegerEx
{
    [ConsumesRNG(0, 1)]
    public static uint GetRandomBit(this uint value, RNG rng)
    {
        Span<uint> setBits = stackalloc uint[32];
        int j = 0;
        for (int i = 0; i < 32; i++)
        {
            uint mask = unchecked((uint)(1 << i));
            if ((value & mask) == mask) setBits[j++] = mask;
        }
        return j switch
        {
            0 => 0,
            1 => setBits[0],
            _ => setBits[rng.IRandom(0, j - 1)]
        };
    }

    [ConsumesRNG(0, 1)]
    public static int GetRandomBit(this int value, RNG rng)
        => unchecked((int)GetRandomBit((uint)value, rng));

    public static int AddClamped(this int value, int addend)
    {
        int result = unchecked(value + addend);
        if ((addend > 0 && result < value) || (addend < 0 && result > value))
            return addend > 0 ? int.MaxValue : int.MinValue;
        return result;
    }

    public static int AddClamped(this int value, int addend, int minValue, int maxValue)
    {
        int result = unchecked(value + addend);
        if ((addend > 0 && result < value) || (addend < 0 && result > value))
            result = addend > 0 ? int.MaxValue : int.MinValue;
        return Math.Clamp(result, minValue, maxValue);
    }
}