using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public abstract class MapContainer
{
    [JsonProperty(PropertyName = "map")]
    public Map2D<Room> Map { get; private set; } = new();

    [JsonProperty(PropertyName = "location")]
    public Point Location;

    [JsonIgnore]
    public Rectangle Bounds => new(Location, Map.Size);

    [JsonIgnore]
    public readonly Dictionary<Guid, Room> Rooms = new();

    public bool TryFindRoom(Guid id, out Room room) => Rooms.TryGetValue(id, out room);

    [OnDeserialized]
    public void OnDeserialize(StreamingContext context) => Populate();

    public void Populate()
    {
        foreach (Room room in Map.Values) Rooms[room.ID] = room;
    }
}