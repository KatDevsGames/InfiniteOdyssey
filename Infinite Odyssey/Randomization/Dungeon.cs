using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class Dungeon
{
    [JsonProperty(PropertyName = "id")]
    public Guid ID;

    [JsonProperty(PropertyName = "seed")]
    public long Seed;

    [JsonProperty(PropertyName = "biome")]
    public Biome Biome;

    [JsonProperty(PropertyName = "level")]
    public int Level;

    [JsonProperty(PropertyName = "boss")]
    public object? Boss;

    [JsonProperty(PropertyName = "entrances")]
    public Transition[] Entrances;

    [JsonProperty(PropertyName = "floors")]
    public Floor[] Floors;
}

public class Floor : MapContainer
{
    [JsonProperty(PropertyName = "id")]
    public Guid ID;

    public IEnumerable<Room> GetNeighbors(Room room)
    {
        foreach (Transition transition in room.Transitions.Values)
        {
            if (transition.State == TransitionState.Unbound) continue;
            yield return transition.DestinationTransition.Room;
        }
    }
}