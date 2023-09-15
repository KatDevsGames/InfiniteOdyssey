namespace InfiniteOdyssey.Extensions;

public class Singleton<T> where T : new()
{
    public static readonly T Instance = new();
}