using System;
using InfiniteOdyssey.Extensions.Converters;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Flags]
[Serializable, JsonConverter(typeof(FlagsEnumConverter<SaveLayout>))]
public enum SaveLayout : uint
{
    None = 0x00,
    Dungeon = 0x01,
    Overworld = 0x02,
    Enterances = 0x04
}