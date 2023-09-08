using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Extensions;

public static class TextureEx
{
    public static void CopyTo(this Texture2D source, Texture2D dest, Rectangle sourceRect, Point destLocation)
    {
        Point size = sourceRect.Size;
        int arraySize = size.X * size.Y;
        Color[] data = new Color[arraySize];
        source.GetData(0, sourceRect, data, 0, arraySize);
        dest.SetData(0, new Rectangle(destLocation, size), data, 0, arraySize);
    }

    public static void CopyTo(this Texture2D source, Texture2D dest, Rectangle sourceRect, Rectangle destRect)
    {
        Point size = sourceRect.Size;
        int width = sourceRect.Width;
        int height = sourceRect.Height;
        int arraySize = size.X * size.Y;
        Color[] data = new Color[arraySize];
        source.GetData(0, sourceRect, data, 0, arraySize);
        int maxX = (destRect.Width - width);
        int maxY = (destRect.Height - height);
        for (int x = 0; x <= maxX; x += width)
        {
            for (int y = 0; y <= maxY; y += height)
            {
                dest.SetData(0, new Rectangle(destRect.Location + new Point(x, y), size), data, 0, arraySize);
            }
        }
    }
}