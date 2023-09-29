using System;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class Treasure
{
    [JsonProperty(PropertyName = "item")]
    public Item Item;

    [JsonIgnore]
    public TreasureTemplate Template;

    private Treasure() { }

    public Treasure(TreasureTemplate template)
    {
        Template = template;
    }
}