using System;
using InfiniteOdyssey.Behaviors.Actors;
using MonoGame.Extended.Collisions;

namespace InfiniteOdyssey.Extensions;

public static class CollisionEventArgsEx
{
    public static IActor GetIActor(this CollisionEventArgs args)
    {
        switch (args.Other)
        {
            case HitBoxSegment seg: return seg.Actor;
            case IActor actor: return actor;
            default: throw new ArgumentException($"Other ICollisionActor must be a {nameof(HitBoxSegment)} or {nameof(IActor)}.", nameof(args.Other));
        }
    }
}