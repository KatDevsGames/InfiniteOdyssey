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

    [JsonProperty(PropertyName = "direction")]
    public Direction4 Direction = Direction4.North;

    [JsonProperty(PropertyName = "exitType")]
    public ExitType ExitType = ExitType.Standard;

    [JsonProperty(PropertyName = "entranceType")]
    public EntranceType EntranceType = EntranceType.NoChange;

    [JsonProperty(PropertyName = "requirements")]
    public List<Requirement> Requirements = new();

    public TransitionTemplate(string name)
    {
        Name = name;
    }
}