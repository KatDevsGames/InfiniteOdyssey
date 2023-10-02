using System;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public enum TransitionState
{
    Unbound = 0,
    Sealed,
    Open,
    Closed,
    Locked
}