using System.Collections.Generic;

namespace InfiniteOdyssey.Extensions;

public class RandomList<T> : List<T>
{
    private readonly RNG _rng;

    public RandomList(RNG rng) => _rng = rng;

    [ConsumesRNG]
    public T RemoveRandom()
    {
        int i = _rng.IRandom(0, Count);
        T result = this[i];
        RemoveAt(i);
        return result;
    }
}