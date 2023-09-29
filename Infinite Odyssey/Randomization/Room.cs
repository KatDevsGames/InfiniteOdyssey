using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class Room
{
    [JsonProperty(PropertyName = "id")]
    public Guid ID;

    [JsonProperty(PropertyName = "seed")]
    public long Seed;

    [JsonProperty(PropertyName = "template")]
    private string TemplateName => Template.Name;

    [JsonIgnore]
    public RoomTemplate Template;

    [JsonIgnore]
    public Point Location;

    [JsonIgnore]
    public Rectangle Bounds => new(Location, Template.Size);

    [JsonProperty(PropertyName = "doorStates")]
    public Dictionary<string, DoorState> DoorStates;

    [JsonProperty(PropertyName = "treasure")]
    public Dictionary<string, object> Treasure;

    [JsonProperty(PropertyName = "enemies")]
    public Dictionary<string, object> Enemies;

    [Serializable]
    public enum DoorState
    {
        Sealed = 0,
        Open,
        Closed,
        Locked,
        Missing
    }

    [JsonConstructor]
    private Room() { }

    public Room(RoomTemplate template)
    {
        Template = template;
    }

    public static Room[][] GetEmptyMap(int width, int height)
    {
        Room[][] map = new Room[width][];
        for (int i = 0; i < width; i++)
            map[i] = new Room[height];
        return map;
    }
}