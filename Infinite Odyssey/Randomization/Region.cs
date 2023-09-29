﻿using System;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class Region : MapContainer
{
    [JsonProperty(PropertyName = "id")]
    public Guid ID;

    [JsonProperty(PropertyName = "seed")]
    public long Seed;

    [JsonProperty(PropertyName = "name")]
    public string Name;

    [JsonProperty(PropertyName = "biome")]
    public Biome Biome;

    [JsonProperty(PropertyName = "level")]
    public int Level;

    [JsonProperty(PropertyName = "dungeons")]
    public Dungeon[] Dungeons;

    [JsonIgnore]
    public RegionParameters Parameters;

    [JsonIgnore]
    public Point SeedLocation;

    public static Region[][] GetEmptyMap(int width, int height)
    {
        Region[][] map = new Region[width][];
        for (int i = 0; i < width; i++)
            map[i] = new Region[height];
        return map;
    }
}