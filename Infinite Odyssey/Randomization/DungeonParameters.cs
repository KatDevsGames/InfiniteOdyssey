using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

public class DungeonParameters
{
    [JsonProperty(PropertyName = "seed")]
    public long? Seed;

    [JsonProperty(PropertyName = "minRooms")]
    public int? MinRooms;

    [JsonProperty(PropertyName = "maxRooms")]
    public int? MaxRooms;

    [JsonProperty(PropertyName = "level")]
    public int? Level;

    [JsonProperty(PropertyName = "dungeonLayout")]
    public DungeonLayout? DungeonLayout;
}