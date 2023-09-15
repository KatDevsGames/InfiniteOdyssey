using System;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class Region : MapContainer
{
    [JsonProperty(PropertyName = "seed")]
    public long Seed;

    [JsonProperty(PropertyName = "biome")]
    public Biome Biome;

    [JsonProperty(PropertyName = "x")]
    public int X;

    [JsonProperty(PropertyName = "y")]
    public int Y;

    [JsonProperty(PropertyName = "level")]
    public int Level;

    [JsonProperty(PropertyName = "dungeons")]
    public Dungeon[] Dungeons;

    [JsonIgnore]
    public RegionParameters Parameters;

    [JsonIgnore]
    public (int x, int y) SeedLocation;

    public static Region[][] GetEmptyMap(int width, int height)
    {
        Region[][] map = new Region[width][];
        for (int i = 0; i < width; i++)
            map[i] = new Region[height];
        return map;
    }
}