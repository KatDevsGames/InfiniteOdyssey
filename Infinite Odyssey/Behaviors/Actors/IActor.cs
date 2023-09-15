using MonoGame.Extended.Collisions;

namespace InfiniteOdyssey.Behaviors.Actors;

public interface IActor
{
    void OnCollision(CollisionEventArgs collisionInfo);
}