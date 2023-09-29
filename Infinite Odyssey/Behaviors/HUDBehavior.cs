using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Behaviors;

public class HUDBehavior : SceneBehavior
{
    public Vector2 Position { get; private set; }

    private readonly Camera.Camera m_camera;

    private readonly Texture2D m_texture;

    public HUDBehavior(Game game, Camera.Camera camera) : base(game)
    {
        m_camera = camera;
        m_texture = new Texture2D(game.GraphicsDevice, Game.NATIVE_RESOLUTION.X, Game.NATIVE_RESOLUTION.Y);
    }

    public override void Update(GameTime gameTime)
    {
        Position = m_camera.Position;
    }

    public override void Draw(GameTime gameTime)
    {
        Game.SpriteBatch.Draw(m_texture, Vector2.Zero, DrawDepth.HUD);
    }
}