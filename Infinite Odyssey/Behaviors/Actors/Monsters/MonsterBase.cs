using InfiniteOdyssey.Scenes.Action;

namespace InfiniteOdyssey.Behaviors.Actors.Monsters;

public abstract class MonsterBase : CreatureBase
{
    public virtual DamageType DamageType => DamageType.None;

    public virtual int BaseDamage => 0;

    protected MonsterBase(ActionScene actionScene) : base(actionScene) { }
}