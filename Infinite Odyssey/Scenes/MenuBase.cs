using System.Runtime.CompilerServices;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Scenes;

public class MenuBase : Scene
{
    private Texture2D m_menuTexture;

    public MenuBase(Game game, bool active = true) : base(game, active) { }

    private static readonly Point TILE_SIZE = new(32, 32);
    private static readonly int TILE_ARRAY_LENGTH = TILE_SIZE.X * TILE_SIZE.Y;
    private static readonly Point SOURCE_FRAME_RED = Point.Zero;
    private static readonly Point SOURCE_FRAME_GREEN = new Point(3, 0) * TILE_SIZE;

    public enum FrameColor
    {
        Red,
        Green
    }

    public override void LoadContent()
    {
        base.LoadContent();
        m_menuTexture = Game.Content.Load<Texture2D>("Sprites\\Menu\\Menu");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Point GetTileOffset(int x, int y) => new(x * TILE_SIZE.X, y * TILE_SIZE.Y);

    public Texture2D GetFrame(FrameColor color, int width, int height)
    {
        Point origin = (color == FrameColor.Red) ? SOURCE_FRAME_RED : SOURCE_FRAME_GREEN;

        Texture2D newTexture = new(Game.GraphicsDevice, width * 32, height * 32);
        Color[] data = new Color[TILE_ARRAY_LENGTH];

        //top left corner
        m_menuTexture.CopyTo(newTexture, new Rectangle(origin, TILE_SIZE), Point.Zero);

        //top line
        m_menuTexture.CopyTo(newTexture, new Rectangle(GetTileOffset(1, 0), TILE_SIZE), new Rectangle(GetTileOffset(1, 0), GetTileOffset(width - 2, 1)));

        //top right corner
        m_menuTexture.CopyTo(newTexture, new Rectangle(origin + GetTileOffset(2, 0), TILE_SIZE), GetTileOffset(width - 1, 0));

        //left line
        m_menuTexture.CopyTo(newTexture, new Rectangle(GetTileOffset(0, 1), TILE_SIZE), new Rectangle(GetTileOffset(0, 1), GetTileOffset(1, height -2)));

        //middle fill
        m_menuTexture.CopyTo(newTexture, new Rectangle(GetTileOffset(1, 1), TILE_SIZE), new Rectangle(GetTileOffset(1, 1), GetTileOffset(width - 2, height - 2)));

        //right line
        m_menuTexture.CopyTo(newTexture, new Rectangle(GetTileOffset(2, 1), TILE_SIZE), new Rectangle(GetTileOffset(width-1, 1), GetTileOffset(1, height - 2)));

        //bottom left corner
        m_menuTexture.CopyTo(newTexture, new Rectangle(origin + GetTileOffset(0, 2), TILE_SIZE), GetTileOffset(0, height - 1));

        //bottom line
        m_menuTexture.CopyTo(newTexture, new Rectangle(GetTileOffset(1, 2), TILE_SIZE), new Rectangle(GetTileOffset(1, height - 1), GetTileOffset(width - 2, 1)));

        //bottom right corner
        m_menuTexture.CopyTo(newTexture, new Rectangle(origin + GetTileOffset(2, 2), TILE_SIZE), GetTileOffset(width - 1, height - 1));
        
        return newTexture;
    }
}