using System;
using InfiniteOdyssey.Behaviors;
using InfiniteOdyssey.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Scenes;

public class Title : Scene
{
    private SpriteFont m_font;

    private PinchCursor m_cursor;

    private int m_cursorPos = 0;

#if DESKTOP
    private readonly string[] m_lines = new string[7];
    private readonly Vector2[] m_lineMeasurements = new Vector2[7];
#else
    private readonly string[] m_lines = new string[6];
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

    public Title(Game game, bool active = true) : base(game, active)
    {
        game.InputMapper.Menu.Up += OnMenuUpDown;
        game.InputMapper.Menu.Down += OnMenuUpDown;
        game.InputMapper.Menu.Confirm += OnMenuConfirm;
        game.InputMapper.Menu.Cancel += OnMenuCancel;
    }

    private void OnMenuUpDown(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!e.Pressed) return;
        switch (e.EventType)
        {
            case InputMapper.MenuEvents.EventTypes.Up:
                CursorUp();
                return;
            case InputMapper.MenuEvents.EventTypes.Down:
                CursorDown();
                return;
        }
    }

    private void OnMenuConfirm(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!e.Pressed) return;
        switch (m_cursorPos)
        {
            case 0: // New Game
                return;
            case 1: // Load Game
                return;
            case 2: // Network Game
                return;
            case 3: // Settings
                Game.SceneManager.Load("Settings");
                return;
            case 4: // Achievements
                return;
            case 5: // Credits
                return;
            case 6: // Quit
                Environment.Exit(0);
                return;
        }
    }

    private void OnMenuCancel(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!e.Pressed) return;
        // do something
    }

    private void CursorUp()
    {
        m_cursorPos--;
        if (m_cursorPos < 0) m_cursorPos = 0;
        SetCursorPos();
    }

    private void CursorDown()
    {
        m_cursorPos++;
        if (m_cursorPos >= m_lines.Length) m_cursorPos = m_lines.Length - 1;
        SetCursorPos();
    }

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

        m_lines[0] = TextLoader.Instance.GetText("TitleMenu", "newGame");
        m_lines[1] = TextLoader.Instance.GetText("TitleMenu", "loadGame");
        m_lines[2] = TextLoader.Instance.GetText("TitleMenu", "networkGame");
        m_lines[3] = TextLoader.Instance.GetText("TitleMenu", "settings");
        m_lines[4] = TextLoader.Instance.GetText("TitleMenu", "achievements");
        m_lines[5] = TextLoader.Instance.GetText("TitleMenu", "credits");
#if DESKTOP
        m_lines[6] = TextLoader.Instance.GetText("TitleMenu", "quit");
#endif

        for (int i = 0; i < m_lines.Length; i++)
            m_lineMeasurements[i] = m_font.MeasureString(m_lines[i]);

        SetCursorPos();
    }

    private void SetCursorPos()
    {
        int position = m_cursorPos;
        Vector2 lineM = m_lineMeasurements[position];
        m_cursor.Y = 100 + (LINE_SPACING * position) + (int)(lineM.Y / 2) + CURSOR_NUDGE_Y;
        m_cursor.Width = (int)lineM.X + 32;
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        for (int i = 0; i < m_lines.Length; i++)
        {
            string line = m_lines[i];
            Game.SpriteBatch.DrawString(m_font, line, new Vector2(100, 100 + (LINE_SPACING * i)), Color.Black);
        }
    }
}