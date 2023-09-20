using System.Collections.Generic;
using InfiniteOdyssey.Extensions;

namespace InfiniteOdyssey.Randomization;

public class Transition
{
    public string Name;
    public Direction4 Direction = Direction4.North;
    public ExitType ExitType = ExitType.Seamless;
    public EntranceType EntranceType = EntranceType.NoChange;
    public List<Requirement> Requirements = new();

    public Transition(string name)
    {
        Name = name;
    }
}