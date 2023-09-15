using InfiniteOdyssey.Randomization;
using InfiniteOdyssey.Scenes;

namespace InfiniteOdyssey.Behaviors.Actors.Items;

public abstract class ItemBase : CreatureBase
{
    public abstract ItemType Type { get; }

    protected ItemBase(ActionScene actionScene) : base(actionScene) { }
}