using InfiniteOdyssey.Extensions;
using Newtonsoft.Json;
using Range = InfiniteOdyssey.Extensions.Range;

namespace InfiniteOdyssey.Randomization;

public class DungeonParameters
{
    [JsonProperty(PropertyName = "seed")]
    public long Seed;

    [JsonProperty(PropertyName = "roomCount")]
    public Range RoomCount;

    [JsonProperty(PropertyName = "floorCount")]
    public Range FloorCount;

    [JsonProperty(PropertyName = "level")]
    public int? Level;

    [JsonProperty(PropertyName = "dungeonLayout")]
    public DungeonLayout? DungeonLayout;

    [JsonProperty(PropertyName = "bossPlacement")]
    public BossPlacement? BossPlacement;

    private static readonly DungeonLayout[] LAYOUT_PRESET_HARDCORE = { Randomization.DungeonLayout.Balanced, Randomization.DungeonLayout.Linear };
    private static readonly DungeonLayout[] LAYOUT_PRESET_CHAOS = { Randomization.DungeonLayout.Balanced, Randomization.DungeonLayout.Linear };
    public static DungeonParameters GetPreset(RNG rng, WorldParameters parameters, RegionParameters regionParameters)
    {
        DungeonParameters dp = new() { Seed = rng.RandomInt64() };
        switch (regionParameters.Preset)
        {
            case Preset.Beginner:
                dp.DungeonLayout = Randomization.DungeonLayout.Sprawling;
                dp.RoomCount = 15..25;
                dp.FloorCount = dp.RoomCount / 7;
                break;
            case Preset.Standard:
                dp.DungeonLayout = Randomization.DungeonLayout.Balanced;
                dp.RoomCount = 25..40;
                dp.FloorCount = dp.RoomCount / rng.IRandom(8..10);
                break;
            case Preset.Hardcore:
                dp.DungeonLayout = LAYOUT_PRESET_HARDCORE.TakeRandom(rng);
                dp.RoomCount = 35..50;
                dp.FloorCount = dp.RoomCount / rng.IRandom(10..14);
                break;
            case Preset.Nightmare:
                dp.DungeonLayout = Randomization.DungeonLayout.Linear;
                dp.RoomCount = 50..70;
                dp.FloorCount = dp.RoomCount / rng.IRandom(10..14);
                break;
            case Preset.Quick:
                dp.DungeonLayout = Randomization.DungeonLayout.Sprawling;
                dp.RoomCount = 15..25;
                dp.FloorCount = dp.RoomCount / 9;
                break;
            case Preset.CompactHard:
                dp.DungeonLayout = Randomization.DungeonLayout.Linear;
                dp.RoomCount = 30..45;
                dp.FloorCount = dp.RoomCount / rng.IRandom(10..14);
                break;
            case Preset.Big:
                dp.DungeonLayout = Randomization.DungeonLayout.Balanced;
                dp.RoomCount = 50..70;
                dp.FloorCount = dp.RoomCount / rng.IRandom(10..14);
                break;
            case Preset.Chaos:
                dp.DungeonLayout = LAYOUT_PRESET_CHAOS.TakeRandom(rng);
                dp.RoomCount = 15..80;
                dp.FloorCount = dp.RoomCount / rng.IRandom(7..15);
                break;
            default:
                goto case Preset.Standard;
        }
        return dp;
    }
}