using System;
using InfiniteOdyssey.Extensions.Converters;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Flags]
[Serializable, JsonConverter(typeof(FlagsEnumConverter<Biome>))]
public enum Biome : uint
{
    Field = 0x01,
    Forest = 0x02,
    Mountain = 0x04,
    Volcano = 0x08,
    Lake = 0x10,
    Ocean = 0x20,
    Desert = 0x40,
    Canyon = 0x80,
    Swamp = 0x100,
    Frozen = 0x200,
    Wasteland = 0x400,
    Ruins = 0x800,
    Cavern = 0x1000,
    Jungle = 0x2000,
    Sky = 0x4000,
    Void = 0x8000,

    Start = 0x0001_0000,
    Chaos = 0x0002_0000,
    Gate = 0x0004_0000,

    Bridge = 0x4000_0000,
    Phlogiston = 0x8000_0000,

    Normal = 0x0000_FFFF
}