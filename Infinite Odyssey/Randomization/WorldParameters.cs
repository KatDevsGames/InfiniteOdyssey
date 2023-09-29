using System;
using InfiniteOdyssey.Extensions;
using Newtonsoft.Json;
using Range = InfiniteOdyssey.Extensions.Range;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class WorldParameters
{
    [JsonProperty(PropertyName = "version")]
    public long? Version;

    [JsonProperty(PropertyName = "seed")]
    public long? Seed;

    [JsonProperty(PropertyName = "preset")]
    public Preset? Preset;

    [JsonProperty(PropertyName = "width")]
    public Range Width;

    [JsonProperty(PropertyName = "height")]
    public Range Height;

    [JsonProperty(PropertyName = "level")]
    public Range Level;

    [JsonProperty(PropertyName = "regions")]
    public RegionParameters?[] Regions;

    [JsonProperty(PropertyName = "biomeDistribution")]
    public BiomeDistribution? BiomeDistribution;

    [JsonProperty(PropertyName = "bossDistribution")]
    public BossDistribution? BossDistribution;

    [JsonProperty(PropertyName = "keyStyle")]
    public KeyStyle? KeyStyle;

    [JsonProperty(PropertyName = "saveLayout")]
    public SaveLayout? SaveLayout;

    public static void InitializeRegions(RNG rng, WorldParameters worldParameters, RegionParameters?[] regions)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            regions[i] ??= RegionParameters.GetPreset(rng, worldParameters);
        }
    }

    public static WorldParameters GetPreset(Preset preset) => GetPreset(preset, new RNG(DateTimeOffset.UtcNow.UtcTicks));

    public static WorldParameters GetPreset(Preset preset, long seed) => GetPreset(preset, new RNG(seed));

    public static WorldParameters GetPreset(Preset preset, RNG rng)
    {
        WorldParameters wp = new() { Seed = rng.RandomInt64() };
        wp.Preset = preset;
        switch (preset)
        {
            case Randomization.Preset.Beginner:
                wp.Width = 8..12;
                wp.Height = 8..12;
                wp.Level = 1..6;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.NoKeys;
                wp.Regions = new RegionParameters[6];
                break;
            case Randomization.Preset.Standard:
                wp.Width = 24..40;
                wp.Height = 24..40;
                wp.Level = 1..8;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[8];
                break;
            case Randomization.Preset.Hardcore:
                wp.Width = 24..40;
                wp.Height = 24..40;
                wp.Level = 4..8;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[8];
                break;
            case Randomization.Preset.Nightmare:
                wp.Width = 40..56;
                wp.Height = 40..56;
                wp.Level = 5..9;
                wp.BossDistribution = Randomization.BossDistribution.RandomNoRepeats;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[8];
                break;
            case Randomization.Preset.Quick:
                wp.Width = 8..12;
                wp.Height = 8..12;
                wp.Level = 1..8;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[6];
                break;
            case Randomization.Preset.CompactHard:
                wp.Width = 8..12;
                wp.Height = 8..12;
                wp.Level = 5..9;
                wp.BossDistribution = Randomization.BossDistribution.RandomNoRepeats;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[6];
                break;
            case Randomization.Preset.Big:
                wp.Width = 56..70;
                wp.Height = 56..70;
                wp.Level = 1..8;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[8];
                break;
            case Randomization.Preset.Chaos:
                wp.Width = 24..40;
                wp.Height = 24..40;
                wp.Level = 1..8;
                wp.BossDistribution = Randomization.BossDistribution.RandomAllowRepeats;
                wp.KeyStyle = Randomization.KeyStyle.Generic;
                wp.Regions = new RegionParameters[8];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
        }
        InitializeRegions(rng, wp, wp.Regions);
        return wp;
    }
}