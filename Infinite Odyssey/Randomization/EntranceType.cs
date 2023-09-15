using System;
using InfiniteOdyssey.Extensions.Converters;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Flags]
[Serializable, JsonConverter(typeof(FlagsEnumConverter<EntranceType>))]
public enum EntranceType
{
    NoChange = 0x00,
    ForceWalking = 0x01,
    Intro = 0x02
}