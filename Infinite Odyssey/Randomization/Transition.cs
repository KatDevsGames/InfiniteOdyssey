using System.Collections.Generic;
using InfiniteOdyssey.Extensions;

namespace InfiniteOdyssey.Randomization;

public class Transition
{
    public int Index;
    public Direction4 Direction = Direction4.North;
    public ExitType ExitType = ExitType.Seamless;
    public EntranceType EntranceType = EntranceType.NoChange;
    public List<Requirement>? Requirements;
}

public struct Requirement
{
    public int TransitionIndex;
    public Item Item;
}