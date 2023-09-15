using System;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class Dungeon : MapContainer
{
    [JsonProperty(PropertyName = "seed")]
    public long Seed;

    [JsonProperty(PropertyName = "biome")]
    public Biome Biome;

    [JsonProperty(PropertyName = "level")]
    public int Level;

    [JsonProperty(PropertyName = "map")]
    public Room[][] Map;

    [JsonProperty(PropertyName = "boss")]
    public object? Boss;
}