using InfiniteOdyssey.Extensions;
using Newtonsoft.Json;
using Range = InfiniteOdyssey.Extensions.Range;

namespace InfiniteOdyssey.Randomization;

public class RegionParameters
{
    [JsonProperty(PropertyName = "seed")]
    public long? Seed;

    [JsonProperty(PropertyName = "preset")]
    public Preset? Preset;

    [JsonProperty(PropertyName = "name")]
    public string? Name;

    [JsonProperty(PropertyName = "biome")]
    public Biome? Biome;

    [JsonProperty(PropertyName = "cellCount")]
    public Range? CellCount;

    [JsonProperty(PropertyName = "level")]
    public int? Level;

    [JsonProperty(PropertyName = "dungeons")]
    public DungeonParameters?[]? Dungeons;

    [JsonProperty(PropertyName = "exitDensity")]
    public float? ExitDensity;

    [JsonProperty(PropertyName = "civilization")]
    public float? Civilization;

    public static void InitializeDungeons(RNG rng, WorldParameters worldParameters, RegionParameters regionParameters, DungeonParameters?[] dungeons)
    {
        for (int i = 0; i < dungeons.Length; i++)
        {
            dungeons[i] ??= DungeonParameters.GetPreset(rng, worldParameters, regionParameters);
        }
    }

    public static RegionParameters GetPreset(RNG rng, WorldParameters parameters)
    {
        RegionParameters rp = new() { Seed = rng.RandomInt64() };
        rp.Biome = SuggestBiome(rng, parameters);
        rp.Preset = parameters.Preset;
        switch (parameters.Preset)
        {
            case Randomization.Preset.Beginner:
                rp.Dungeons = new DungeonParameters[1];
                break;
            case Randomization.Preset.Standard:
                rp.Dungeons = new DungeonParameters[1];
                break;
            case Randomization.Preset.Hardcore:
                rp.Dungeons = new DungeonParameters[rng.IRandom(1, 3)];
                break;
            case Randomization.Preset.Nightmare:
                rp.Dungeons = new DungeonParameters[rng.IRandom(1, 3, 1.5)];
                break;
            case Randomization.Preset.Quick:
                rp.Dungeons = new DungeonParameters[rng.IRandom(0, 1, 2)];
                break;
            case Randomization.Preset.CompactHard:
                rp.Dungeons = new DungeonParameters[rng.IRandom(1, 2)];
                break;
            case Randomization.Preset.Big:
                rp.Dungeons = new DungeonParameters[rng.IRandom(1, 2, 1.75)];
                break;
            case Randomization.Preset.Chaos:
                rp.Dungeons = new DungeonParameters[rng.IRandom(1, 4, 0.8)];
                break;
            default:
                goto case Randomization.Preset.Standard;
        }
        InitializeDungeons(rng, parameters, rp, rp.Dungeons);
        return rp;
    }

    [ConsumesRNG]
    private static RegionParameters SuggestRegion(RNG rng, WorldParameters parameters)
    {
        RegionParameters result = new();
        result.Biome ??= SuggestBiome(rng, parameters);
        return result;
    }

    [ConsumesRNG]
    private static Biome SuggestBiome(RNG rng, WorldParameters parameters)
    {
        switch (parameters.BiomeDistribution)
        {
            case BiomeDistribution.RandomNoRepeats:
                Biome remainingBiomes = Randomization.Biome.Normal;
                foreach (RegionParameters? region in parameters.Regions)
                {
                    if (region?.Biome != null) remainingBiomes &= (~region.Biome.Value);
                }
                if (remainingBiomes == 0) goto case BiomeDistribution.RandomAllowRepeats;
                return (Biome)((uint)remainingBiomes).GetRandomBit(rng);
            case BiomeDistribution.RandomAllowRepeats:
                return (Biome)((uint)Randomization.Biome.Normal).GetRandomBit(rng);
            default:
                goto case BiomeDistribution.RandomNoRepeats;
        }
    }
}