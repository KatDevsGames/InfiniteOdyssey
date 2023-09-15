#if DEBUG
using InfiniteOdyssey.Extensions;
using InfiniteOdyssey.Randomization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Scenes;

public class MapPreviewScene : Scene
{
    private Texture2D m_texture;

    public MapPreviewScene(Game game, bool active = true) : base(game, active) { }

    public override void LoadContent()
    {
        m_texture = new Texture2D(Game.GraphicsDevice, Game.NATIVE_RESOLUTION.X, Game.NATIVE_RESOLUTION.Y);
        Region?[][] map = Game.State.World.RegionMap;
        int width = map.Length;
        int height = map[0].Length;

        int cellWidth = m_texture.Width / width;
        int cellHeight = m_texture.Height / height;
        int cellSize = cellWidth * cellHeight;

        Color[] cellToPaint = new Color[cellSize];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Rectangle rect = new(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                cellToPaint.Fill(map[x][y]?.Biome.GetDebugColor() ?? Color.Magenta);
                m_texture.SetData(0, rect, cellToPaint, 0, cellSize);
            }
        }

        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        Game.SpriteBatch.Draw(m_texture, Vector2.Zero, Color.White);
        base.Draw(gameTime);
    }
}
#endif