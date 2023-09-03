namespace InfiniteOdyssey.Extensions;

internal class Singleton<T> where T : new()
{
    public static readonly T Instance = new();
}