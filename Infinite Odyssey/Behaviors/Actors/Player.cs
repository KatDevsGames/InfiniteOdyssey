using System.Collections.Generic;
using InfiniteOdyssey.Behaviors.Actors.Items;
using InfiniteOdyssey.Behaviors.Actors.Monsters;
using InfiniteOdyssey.Extensions;
using InfiniteOdyssey.Scenes;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace InfiniteOdyssey.Behaviors.Actors;

public class Player : CreatureBase
{
    private static readonly Size2 BODY_HITBOX_SIZE = new(14, 30);
    private static readonly Point2 BODY_HITBOX_MARGIN = new(1, 1);

    public Player(ActionScene actionScene) : base(actionScene) { }

    protected override IList<HitBoxSegment> CreateHitBoxes() => new HitBoxSegment[] { new(this, new RectangleF(Point2.Zero, BODY_HITBOX_SIZE)) };

    protected override void AdjustHitBoxes() => HitBoxes[0].Bounds.Position = Position.Add(BODY_HITBOX_MARGIN);

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        if(collisionInfo.Other is not HitBoxSegment other) return;
        switch (other.Actor)
        {
            case MonsterBase monster:
            {
                TakeDamage(monster);
                break;
            }
            case ItemBase item:
            {
                break;
            }
        }
    }

    public void TakeDamage(MonsterBase monster)
    {

    }
}