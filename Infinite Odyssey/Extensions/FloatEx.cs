namespace InfiniteOdyssey.Extensions;

public static class FloatEx
{
    public static void Truncate(ref this float value, float limit)
    {
        if (value > limit) value = limit;
        if (value < (-limit)) value = -limit;
    }
}