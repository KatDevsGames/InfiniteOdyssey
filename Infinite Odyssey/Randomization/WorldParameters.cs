using System;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

public class WorldParameters
{
    [JsonProperty(PropertyName = "version")]
    public long? Version;

    [JsonProperty(PropertyName = "seed")]
    public long? Seed;

    [JsonProperty(PropertyName = "minWidth")]
    public int MinWidth;

    [JsonProperty(PropertyName = "maxWidth")]
    public int MaxWidth;

    [JsonProperty(PropertyName = "minHeight")]
    public int MinHeight;

    [JsonProperty(PropertyName = "maxHeight")]
    public int MaxHeight;

    [JsonProperty(PropertyName = "minLevel")]
    public int MinLevel;

    [JsonProperty(PropertyName = "maxLevel")]
    public int MaxLevel;

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

    public static WorldParameters GetPreset(Preset preset, long? seed = null)
    {
        WorldParameters wp = new() { Seed = seed };
        switch (preset)
        {
            case Preset.Beginner:
                wp.MinWidth = 8;
                wp.MinHeight = 8;
                wp.MaxWidth = 12;
                wp.MaxHeight = 12;
                wp.MinLevel = 1;
                wp.MaxLevel = 6;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.NoKeys;
                wp.Regions = new RegionParameters[6];
                break;
            case Preset.Standard:
                wp.MinWidth = 24;
                wp.MinHeight = 24;
                wp.MaxWidth = 40;
                wp.MaxHeight = 40;
                wp.MinLevel = 1;
                wp.MaxLevel = 8;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[8];
                break;
            case Preset.Hardcore:
                wp.MinWidth = 24;
                wp.MinHeight = 24;
                wp.MaxWidth = 40;
                wp.MaxHeight = 40;
                wp.MinLevel = 4;
                wp.MaxLevel = 8;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[8];
                break;
            case Preset.Nightmare:
                wp.MinWidth = 40;
                wp.MinHeight = 40;
                wp.MaxWidth = 56;
                wp.MaxHeight = 56;
                wp.MinLevel = 5;
                wp.MaxLevel = 9;
                wp.BossDistribution = Randomization.BossDistribution.RandomNoRepeats;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[8];
                break;
            case Preset.Quick:
                wp.MinWidth = 8;
                wp.MinHeight = 8;
                wp.MaxWidth = 12;
                wp.MaxHeight = 12;
                wp.MinLevel = 1;
                wp.MaxLevel = 8;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[6];
                break;
            case Preset.CompactHard:
                wp.MinWidth = 8;
                wp.MinHeight = 8;
                wp.MaxWidth = 12;
                wp.MaxHeight = 12;
                wp.MinLevel = 5;
                wp.MaxLevel = 9;
                wp.BossDistribution = Randomization.BossDistribution.RandomNoRepeats;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[6];
                break;
            case Preset.Big:
                wp.MinWidth = 56;
                wp.MinHeight = 56;
                wp.MaxWidth = 70;
                wp.MaxHeight = 70;
                wp.MinLevel = 1;
                wp.MaxLevel = 8;
                wp.BossDistribution = Randomization.BossDistribution.ByBiome;
                wp.KeyStyle = Randomization.KeyStyle.DungeonRestricted;
                wp.Regions = new RegionParameters[8];
                break;
            case Preset.Chaos:
                wp.MinWidth = 24;
                wp.MinHeight = 24;
                wp.MaxWidth = 40;
                wp.MaxHeight = 40;
                wp.MinLevel = 1;
                wp.MaxLevel = 8;
                wp.BossDistribution = Randomization.BossDistribution.RandomAllowRepeats;
                wp.KeyStyle = Randomization.KeyStyle.Generic;
                wp.Regions = new RegionParameters[8];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(preset), preset, null);
        }
        return wp;
    }
}