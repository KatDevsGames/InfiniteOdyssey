using System;
using InfiniteOdyssey.Extensions.Converters;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Flags]
[Serializable, JsonConverter(typeof(FlagsEnumConverter<AttachmentPreference>))]
public enum AttachmentPreference
{
    Any = 0x00,
    MatchSize = 0x01,
    MatchExits = 0x02,
    RequireSize = MatchSize | Required,
    RequireExits = MatchExits | Required,
    Detached = 0x04,
    RequireDetached = Detached | Required,
    Required = 0x80
}