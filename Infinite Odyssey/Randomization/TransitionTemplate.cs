using System.Collections.Generic;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

public class TransitionTemplate
{
    [JsonProperty(PropertyName = "name")]
    public string Name;

    [JsonProperty(PropertyName = "roomTemplate")]
    public string RoomTemplate;

    [JsonProperty(PropertyName = "bounds")]
    public Rectangle Bounds;

    [JsonIgnore]
    public Point Location => Bounds.Center / Game.PIXELS_PER_SCREEN;

    [JsonProperty(PropertyName = "direction")]
    public Direction4 Direction = Direction4.North;

    [JsonProperty(PropertyName = "exitType")]
    public ExitType ExitType = ExitType.Open;

    [JsonProperty(PropertyName = "entranceType")]
    public EntranceType EntranceType = EntranceType.NoChange;

    [JsonProperty(PropertyName = "requirements")]
    public List<Requirement> Requirements = new();

    public TransitionTemplate(string name)
    {
        Name = name;
    }

    public bool IsMatch(TransitionTemplate? other)
    {
        if (other == null) return false;
        if (ExitType != other.ExitType) return false;
        return Direction == other.Direction.GetOpposite();
    }
}