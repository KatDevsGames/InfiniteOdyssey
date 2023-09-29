using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InfiniteOdyssey.Behaviors.Actors.Monsters;
using InfiniteOdyssey.Scenes.Action;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace InfiniteOdyssey.Behaviors.Actors;

public abstract class CreatureBase : SceneBehavior, IActor
{
    private readonly ActionScene m_actionScene;
    private Point2 m_position;

    public int Health { get; private set; }

    public Point2 Position
    {
        get => m_position;
        set
        {
            m_position = value;
            AdjustHitBoxes();
        }
    }

    public abstract Size2 Size { get; }

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

    public virtual float GetDamageMultiplier(DamageType type) => 1f;
    public virtual void TakeDamage(DamageType type, int amount)
    {
        Health -= (int)(amount * GetDamageMultiplier(type));
    }

    public override void Draw(GameTime gameTime)
    {
#if DEBUG
        foreach (HitBoxSegment segment in HitBoxes)
            Game.SpriteBatch.DrawRectangle((RectangleF)segment.Bounds, Color.Red, 1);
#endif
        base.Draw(gameTime);
    }
}