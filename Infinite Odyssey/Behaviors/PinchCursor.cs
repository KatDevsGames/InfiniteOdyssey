using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Behaviors;

public class PinchCursor : SceneBehavior
{
    public int X { get; set; }

    public int Y
    {
        get => m_sprite.Y;
        set => m_sprite.Y = value;
    }

    public int Width { get; set; }

    private SpriteBehavior m_sprite;

    public PinchCursor(Game game) : base(game) { }

    public override void LoadContent()
    {
        m_sprite = new(Game, "Sprites\\Menu\\PinchCursor", 16, 16, new[] { 0, 1 }, 0.2, true);
    }

    public override void Update(GameTime gameTime)
    {
        m_sprite.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        m_sprite.X = X;
        m_sprite.FlipX = false;
        m_sprite.Draw(gameTime);

        m_sprite.X += Width;
        m_sprite.FlipX = true;
        m_sprite.Draw(gameTime);
    }
}