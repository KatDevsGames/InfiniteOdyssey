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

    [JsonProperty(PropertyName = "transitions")]
    public Dictionary<string, Transition> Transitions;

    [JsonProperty(PropertyName = "treasure")]
    public Dictionary<string, object> Treasure;

    [JsonProperty(PropertyName = "enemies")]
    public Dictionary<string, object> Enemies;

    [JsonConstructor]
    private Room() { }

    public Room(RoomTemplate template)
    {
        Template = template;
        Transitions = new();
        foreach (var transition in template.Transitions.Values)
        {
            Transitions.Add(transition.Name, new(this, transition));
        }
    }
}