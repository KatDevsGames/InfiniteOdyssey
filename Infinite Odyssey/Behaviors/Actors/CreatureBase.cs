using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InfiniteOdyssey.Scenes;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace InfiniteOdyssey.Behaviors.Actors;

public abstract class CreatureBase : SceneBehavior, IActor
{
    private readonly ActionScene m_actionScene;
    private Point2 m_position;

    public Point2 Position
    {
        get => m_position;
        set
        {
            m_position = value;
            AdjustHitBoxes();
        }
    }

    public IList<HitBoxSegment> HitBoxes { get; }

    [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor")]
    protected CreatureBase(ActionScene actionScene) : base(actionScene.Game)
    {
        m_actionScene = actionScene;
        HitBoxes = CreateHitBoxes();
        AdjustHitBoxes();
    }

    protected virtual IList<HitBoxSegment> CreateHitBoxes() => Array.Empty<HitBoxSegment>();

    protected virtual void AdjustHitBoxes() { }

    public virtual void OnCollision(CollisionEventArgs collisionInfo) { }

    public override void Draw(GameTime gameTime)
    {
#if DEBUG
        foreach (HitBoxSegment segment in HitBoxes)
            Game.SpriteBatch.DrawRectangle((RectangleF)segment.Bounds, Color.Red, 3);
#endif
        base.Draw(gameTime);
    }
}