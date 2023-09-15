using System;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public enum ExitType
{
    Seamless,
    Scroll,
    Door,
    LockedDoor,
    AreaChange,
    Mirror,
    Scripted
}