using System;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class Room
{
    [JsonProperty(PropertyName = "seed")]
    public long Seed;

    [JsonProperty(PropertyName = "level")]
    public int Level;

    [JsonIgnore]
    public int X;

    [JsonIgnore]
    public int Y;

    [JsonIgnore]
    public int Width;

    [JsonIgnore]
    public int Height;

    /// <summary>
    /// The name of the tilemap that this cell uses.
    /// </summary>
    [JsonProperty(PropertyName = "tileMap")]
    public string TileMap;

    /// <summary>
    /// These correspond to any pickups in the room be they freestanding, hidden, or in chests.
    /// They are indexed as they appear in the room, starting at the upper-leftmost cell, scanning rightward,
    /// and, when the end of a line is reached, moving to the leftmost-cell of the next lower line.
    /// (Objects are scanned much in the way English-language text is normally read.)
    /// </summary>
    [JsonProperty(PropertyName = "items")]
    public object[] Items;

    /// <summary>
    /// Each 23x40 map cell along the edge of the room may contain doors.
    /// These go clockwise, starting on the leftmost cell along the top wall.
    /// A zero-or-null value denotes the lack of a door.//todo zero-or-null
    /// There is one entry for every cell-span along each wall, centered within that cell-span.
    /// (e.g. A 4x3 room will contain 14 entries. 4 left-to-right along the top wall followed by
    /// 3 top-to-bottom along the right wall followed by 4 right-to-left along the bottom wall
    /// followed by, 3 bottom-to-top along the left wall.)
    /// </summary>
    [JsonProperty(PropertyName = "doorStates")]
    public object?[] DoorStates;

    public static Room[][] GetEmptyMap(int width, int height)
    {
        Room[][] map = new Room[width][];
        for (int i = 0; i < width; i++)
            map[i] = new Room[height];
        return map;
    }
}