using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class TreasureTemplate
{
    [JsonProperty(PropertyName = "name")]
    public string Name;

    [JsonProperty(PropertyName = "bounds")]
    public Rectangle Bounds;

    [JsonProperty(PropertyName = "requirements")]
    public List<Requirement> Requirements = new();

    public TreasureTemplate(string name)
    {
        Name = name;
    }
}