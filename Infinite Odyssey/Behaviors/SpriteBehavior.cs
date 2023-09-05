using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Behaviors;

public class SpriteBehavior : SceneBehavior
{
    public Texture2D Texture { get; }

    public int X { get; set; }

    public int Y { get; set; }

    public bool FlipX { get; set; }

    public bool FlipY { get; set; }

    public float Depth { get; set; }

    public Vector2 Scale { get; set; } = Vector2.One;

    public Color Color { get; set; } = Color.White;

    public float Rotation { get; set; } = 0f;

    public int RegionWidth { get; }

    public int RegionHeight { get; }

    public int[] Frames { get; }

    public double FrameDuration { get; }

    public bool Looping { get; }

    public bool Running { get; set; } = true;

    public double AnimationLength => Frames.Length * FrameDuration;

    private Rectangle m_sourceRect;

    private double m_deltaTime = 0;

    private readonly int m_rows;
    private readonly int m_cols;

    private bool m_firstCycle;

    public SpriteBehavior(Game game, string assetName, int regionWidth, int regionHeight, int[] frames, double frameDuration, bool looping)
        :this(game, game.Content.Load<Texture2D>(assetName), regionWidth, regionHeight, frames, frameDuration, looping){}


    public SpriteBehavior(Game game, Texture2D texture, int regionWidth, int regionHeight, int[] frames, double frameDuration, bool looping) : base(game)
    {
        Texture = texture;
        RegionWidth = regionWidth;
        RegionHeight = regionHeight;
        Frames = frames;
        FrameDuration = frameDuration;
        Looping = looping;

        m_rows = texture.Height / regionHeight;
        m_cols = texture.Width / regionWidth;
    }

    public override void Update(GameTime gameTime)
    {
        if (!Running) return;
        if (!Looping && m_firstCycle) return;
        m_deltaTime += gameTime.ElapsedGameTime.TotalSeconds;
        double al = AnimationLength;
        while (m_deltaTime > al)
        {
            m_deltaTime -= al;
            m_firstCycle = false;
        }

        int cycleOffset = (int)(m_deltaTime / FrameDuration);

        int f = Frames[cycleOffset];
        int x = f % m_cols;
        int y = f / m_cols;
        m_sourceRect = new Rectangle(x, y, RegionWidth, RegionHeight);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteEffects se = SpriteEffects.None;
        if (FlipX) se |= SpriteEffects.FlipHorizontally;
        if (FlipY) se |= SpriteEffects.FlipVertically;
        Game.SpriteBatch.Draw(Texture, new Vector2(X, Y), m_sourceRect, Color, Rotation, Vector2.Zero, Scale, se, Depth);
    }
}