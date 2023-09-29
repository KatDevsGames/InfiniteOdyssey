using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Extensions;

public static class SpriteBatchEx
{
    public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, DrawDepth drawDepth)
        => spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, (float)drawDepth);

    public static void DrawString(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, DrawDepth drawDepth)
        => spriteBatch.DrawString(spriteFont, text, position, Color.White, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, (float)drawDepth);
   
    public static void DrawString(this SpriteBatch spriteBatch, SpriteFont spriteFont, string text, Vector2 position, Color color, DrawDepth drawDepth)
        => spriteBatch.DrawString(spriteFont, text, position, color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, (float)drawDepth);
}