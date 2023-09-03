using InfiniteOdyssey.Behaviors;
using InfiniteOdyssey.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Scenes;

public class Title : Scene
{
    private SpriteFont m_font;

    private PinchCursor m_cursor;

#if DESKTOP
    private readonly string[] m_titleLines = new string[7];
    private readonly Vector2[] m_lineMeasurements = new Vector2[7];
#else
    private readonly string[] m_titleLines = new string[6];
    private readonly Vector2[] m_lineMeasurements = new Vector2[6];
#endif
    // "newGame": "New Game",
    // "loadGame": "Load Game",
    // "networkGame": "Network Game",
    // "settings": "Settings",
    // "achievements": "Achievements",
    // "credits": "Credits",
    // "quit": "Quit"

    private const int LINE_SPACING = 50;

    private const int CURSOR_NUDGE_Y = -8;

    public Title(Game game) : base(game) { }

    public override void Initialize()
    {
        AddBehavior("Cursor", m_cursor = new PinchCursor(Game) { X = (100 - 24), Width = 100 });
    }

    public override void LoadContent()
    {
        base.LoadContent();
        //Sound sound = CoreSystem.LoadStreamedSound("Music\\Title.ogg");
        //Channel channel = sound.Play();
        //channel.Looping = true;

        m_font = Game.Content.Load<SpriteFont>("Fonts\\TitleMenu");

        m_titleLines[0] = TextLoader.Instance.GetText("TitleMenu", "newGame");
        m_titleLines[1] = TextLoader.Instance.GetText("TitleMenu", "loadGame");
        m_titleLines[2] = TextLoader.Instance.GetText("TitleMenu", "networkGame");
        m_titleLines[3] = TextLoader.Instance.GetText("TitleMenu", "settings");
        m_titleLines[4] = TextLoader.Instance.GetText("TitleMenu", "achievements");
        m_titleLines[5] = TextLoader.Instance.GetText("TitleMenu", "credits");
#if DESKTOP
        m_titleLines[6] = TextLoader.Instance.GetText("TitleMenu", "quit");
#endif

        for (int i = 0; i < m_titleLines.Length; i++)
            m_lineMeasurements[i] = m_font.MeasureString(m_titleLines[i]);

        SetCursorPos(0);
    }

    private void SetCursorPos(int position)
    {
        Vector2 lineM = m_lineMeasurements[position];
        m_cursor.Y = 100 + (LINE_SPACING * position) + (int)(lineM.Y / 2) + CURSOR_NUDGE_Y;
        m_cursor.Width = (int)lineM.X + 24;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        for (int i = 0; i < m_titleLines.Length; i++)
        {
            string line = m_titleLines[i];
            Game.SpriteBatch.DrawString(m_font, line, new Vector2(100, 100 + (LINE_SPACING * i)), Color.Black);
        }
    }
}