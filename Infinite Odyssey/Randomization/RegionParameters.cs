using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

public class RegionParameters
{
    [JsonProperty(PropertyName = "seed")]
    public long? Seed;

    [JsonProperty(PropertyName = "biome")]
    public Biome? Biome;

    [JsonProperty(PropertyName = "minCells")]
    public int? MinCells;

    [JsonProperty(PropertyName = "maxCells")]
    public int? MaxCells;

    [JsonProperty(PropertyName = "level")]
    public int? Level;

    [JsonProperty(PropertyName = "dungeons")]
    public DungeonParameters?[]? Dungeons;

    [JsonProperty(PropertyName = "exitDensity")]
    public float? ExitDensity;

    [JsonProperty(PropertyName = "civilization")]
    public float? Civilization;
}