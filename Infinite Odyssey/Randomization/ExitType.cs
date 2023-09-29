using System;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public enum ExitType
{
    Standard = 0,
    Door,
    LockedDoor,
    Dungeon,
    Mirror,
    Scripted
}