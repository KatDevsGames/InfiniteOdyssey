using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace InfiniteOdyssey.Behaviors.Actors;

public class HitBoxSegment : ICollisionActor
{
    public IActor Actor { get; }

    public IShapeF Bounds { get; }

    public HitBoxSegment(IActor actor, IShapeF bounds)
    {
        Actor = actor;
        Bounds = bounds;
    }

    public void OnCollision(CollisionEventArgs collisionInfo) => Actor.OnCollision(collisionInfo);
}