﻿#if DEBUG
using InfiniteOdyssey.Extensions;
using InfiniteOdyssey.Randomization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Scenes.MapPreview;

public class MapPreviewScene : Scene
{
    private readonly Rectangle m_screenRectangle;

    private Texture2D m_texture;

    private SpriteFont m_font;
    private string m_seed;

    public MapPreviewScene(Game game, bool active = true) : base(game, active)
    {
        m_screenRectangle = new Rectangle(new Point(0, 0), Game.NATIVE_RESOLUTION);
    }

    public override void LoadContent()
    {
        m_font = Game.Content.Load<SpriteFont>("Fonts\\SettingsMenu");
        m_seed = Game.State.Parameters.Seed.ToString();

        var map = Game.State.World.RegionMap;
        int width = map.Size.X;
        int height = map.Size.Y;

        int cellWidth = Game.NATIVE_RESOLUTION.X / width;
        int cellHeight = Game.NATIVE_RESOLUTION.Y / height;
        int cellSize = cellWidth * cellHeight;

        m_texture = new Texture2D(Game.GraphicsDevice, cellWidth * width, cellHeight * height);

        Color[] cellToPaint = new Color[cellSize];
        foreach (int x in map.Range)
        {
            var col = map[x];
            foreach (int y in col.Range)
            {
                Rectangle rect = new(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                cellToPaint.Fill(col[y]?.Biome.GetDebugColor() ?? Color.Magenta);
                m_texture.SetData(0, rect, cellToPaint, 0, cellSize);
            }
        }

        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        Game.SpriteBatch.Draw(m_texture, m_screenRectangle, Color.White);
        Game.SpriteBatch.DrawString(m_font, m_seed, new Vector2(16, 16), Color.White);
        Game.SpriteBatch.DrawString(m_font, m_seed, new Vector2(12, 12), Color.Black);
        base.Draw(gameTime);
    }
}
#endif