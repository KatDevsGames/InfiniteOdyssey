using System.Collections.Generic;
using InfiniteOdyssey.Behaviors.Actors.Items;
using InfiniteOdyssey.Behaviors.Actors.Monsters;
using InfiniteOdyssey.Extensions;
using InfiniteOdyssey.Scenes.Action;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace InfiniteOdyssey.Behaviors.Actors;

public class Player : CreatureBase
{
    private static readonly Size2 BODY_HITBOX_SIZE = new(30, 46);
    private static readonly Point2 BODY_HITBOX_MARGIN = new(1, 1);

    public override Size2 Size => new(32, 48);

    public Player(ActionScene actionScene) : base(actionScene) { }

    protected override IList<HitBoxSegment> CreateHitBoxes() => new HitBoxSegment[] { new(this, new RectangleF(Point2.Zero, BODY_HITBOX_SIZE)) };

    protected override void AdjustHitBoxes() => HitBoxes[0].Bounds.Position = Position.Add(BODY_HITBOX_MARGIN);

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if (collisionInfo.Other is not HitBoxSegment other) return;
        switch (other.Actor)
        {
            case MonsterBase monster:
            {

                break;
            }
            case ItemBase item:
            {
                Collect(item);
                break;
            }
        }
    }

    public void Collect(ItemBase item)
    {
        Game.State.PlayerState.TryAddItem(item.Item);
    }
}