﻿using InfiniteOdyssey.Extensions;
using MonoGame.Extended.Tiled;

namespace InfiniteOdyssey.Randomization;

public class Variation
{
    public string Name;
    public TiledMapLayer Layer;
    public Range Level;
    public float Probability;
}