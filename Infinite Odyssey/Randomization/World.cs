using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class World
{
    [JsonProperty(PropertyName = "id")]
    public Guid ID;

    [JsonProperty(PropertyName = "seed")]
    public long Seed;

    [JsonProperty(PropertyName = "regions")]
    public Region[] Regions;

    [JsonIgnore]
    public readonly Map2D<Region> RegionMap = new();

    [JsonIgnore]
    public readonly Map2D<Room> RoomMap = new();

    [JsonIgnore]
    public int Width;

    [JsonIgnore]
    public int Height;

    [OnDeserialized]
    public void OnDeserialize(StreamingContext context) => Populate();

    public void Populate()
    {
        foreach (Region region in Regions)
        {
            Width = Math.Max(Width, (region.Bounds.X + region.Bounds.Width) - 1);
            Height = Math.Max(Height, (region.Bounds.Y + region.Bounds.Height) - 1);
        }

        foreach (Region region in Regions)
        {
            var map = region.Map;
            foreach (int x in map.Range)
            {
                var col = map[x];
                foreach (int y in col.Range)
                {
                    int cellX = x + region.Location.X;
                    int cellY = y + region.Location.Y;
                    RegionMap[cellX][cellY] = region;
                    RoomMap[cellX][cellY] = col[y];
                }
            }
        }
    }
}