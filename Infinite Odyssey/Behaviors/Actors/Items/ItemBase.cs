using InfiniteOdyssey.Randomization;
using InfiniteOdyssey.Scenes.Action;

namespace InfiniteOdyssey.Behaviors.Actors.Items;

public abstract class ItemBase : CreatureBase
{
    public abstract Item Item { get; }

    protected ItemBase(ActionScene actionScene) : base(actionScene) { }
}