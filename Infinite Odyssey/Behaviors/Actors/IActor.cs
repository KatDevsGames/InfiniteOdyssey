using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace InfiniteOdyssey.Behaviors.Actors;

public interface IActor
{
    Point2 Position { get; set; }

    Size2 Size { get; }

    void OnCollision(CollisionEventArgs collisionInfo);
}