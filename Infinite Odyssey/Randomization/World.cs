using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class World
{
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
            Width = Math.Max(Width, (region.X + region.Width) - 1);
            Height = Math.Max(Height, (region.Y + region.Height) - 1);
        }

        RegionMap = Region.GetEmptyMap(Width, Height);
        RoomMap = Room.GetEmptyMap(Width, Height);

        foreach (Region region in Regions)
        {
            Room[][] map = region.Map;
            for (int x = 0; x < map.Length; x++)
            {
                Room[] row = map[x];
                for (int y = 0; y < row.Length; y++)
                {
                    int cellX = x + region.X;
                    int cellY = y + region.Y;
                    RegionMap[cellX][cellY] = region;
                    RoomMap[cellX][cellY] = row[y];
                }
            }
        }
    }
}