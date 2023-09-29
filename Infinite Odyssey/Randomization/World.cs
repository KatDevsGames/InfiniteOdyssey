using System;
using System.Collections.Generic;
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
    public Region?[][] RegionMap;

    [JsonIgnore]
    public Room?[][] RoomMap;

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

        RegionMap = Region.GetEmptyMap(Width, Height);
        RoomMap = Room.GetEmptyMap(Width, Height);

        foreach (Region region in Regions)
        {
            List<List<Room?>> map = region.Map;
            for (int x = 0; x < map.Count; x++)
            {
                List<Room?> row = map[x];
                for (int y = 0; y < row.Count; y++)
                {
                    int cellX = x + region.Bounds.X;
                    int cellY = y + region.Bounds.Y;
                    RegionMap[cellX][cellY] = region;
                    RoomMap[cellX][cellY] = row[y];
                }
            }
        }
    }
}