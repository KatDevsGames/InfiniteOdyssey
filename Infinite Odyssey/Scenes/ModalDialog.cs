using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Behaviors;
using InfiniteOdyssey.Extensions;
using InfiniteOdyssey.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace InfiniteOdyssey.Scenes;

public class ModalDialog : MenuBase
{
    private SpriteFont m_font;

    private PinchCursor m_cursor;

    private int m_cursorPos = 0;

    private Texture2D m_background;
    private Vector2 m_backgroundPosition;

    private TextLoader m_textLoader;
    private readonly List<TextLoader.Locale> m_locales = TextLoader.Manifest.Locales.Values.ToList();
    private int m_selectedLocaleIndex;
    private TextLoader.Locale m_selectedLocale;
    private string? m_selectedLocaleName;

    private readonly DialogType m_type;
    private readonly DialogResult m_cancelValue;

    private readonly string[] m_lines = new string[4];
    private readonly Vector2[] m_lineMeasurements = new Vector2[4];
    private readonly int[] m_lineOffsets = new int[4];

    private const int OPTION_SPACING = 50;

    private const int CURSOR_NUDGE_X = -8;

    private static readonly Point BACKGROUND_MARGIN = new(32, 16);

    public enum DialogResult
    {
        No = 0,
        Yes = 1,
        Cancel = 2,
        Confirm = 3
    }

    public enum DialogType
    {
        YesNo = 0,
        ConfirmCancel = 1,
        Confirm = 2
    }

    private static readonly int[][] OPTION_INDEXES = new[]
    {
        new []{ (int)DialogResult.Yes, (int)DialogResult.No },
        new []{ (int)DialogResult.Confirm, (int)DialogResult.Cancel },
        new []{ (int)DialogResult.Confirm },
    };

    private readonly int[] m_optionIndexes;

    public ModalDialog(Game game, DialogType type, DialogResult cancelValue, bool active = true) : base(game, active)
    {
        m_type = type;
        m_optionIndexes = OPTION_INDEXES[(int)type];
        m_cancelValue = cancelValue;

        m_textLoader = TextLoader.Instance;
        
        game.InputMapper.Menu.Left += OnMenuLeftRight;
        game.InputMapper.Menu.Right += OnMenuLeftRight;
        game.InputMapper.Menu.Confirm += OnMenuConfirm;
        game.InputMapper.Menu.Cancel += OnMenuCancel;
    }

    private void OnMenuLeftRight(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!e.Pressed) return;
        switch (e.EventType)
        {
            case InputMapper.MenuEvents.EventTypes.Left:
                CycleValue(ref m_cursorPos, -1, m_optionIndexes.Length);
                return;
            case InputMapper.MenuEvents.EventTypes.Right:
                CycleValue(ref m_cursorPos, 1, m_optionIndexes.Length);
                return;
        }
        SetCursorPos();
    }

    private void CycleValue(ref int value, int change, int max)
    {
        value += change;
        if (value < 0) value = max - 1;
        else if (value >= max) value = 0;
    }

    private void OnMenuConfirm(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!e.Pressed) return;
        Game.SceneManager.Return((DialogResult)m_cursorPos);
    }

    private void OnMenuCancel(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!e.Pressed) return;
        m_cursorPos = m_optionIndexes.IndexOf((int)m_cancelValue);
        SetCursorPos();
    }

    public override void Initialize()
    {
        AddBehavior("Cursor", m_cursor = new PinchCursor(Game));
    }

    public override void LoadContent()
    {
        base.LoadContent();

        m_font = Game.Content.Load<SpriteFont>("Fonts\\ModalDialog");
        m_background = GetFrame(FrameColor.Red, 15, 5);
        m_backgroundPosition = new Vector2((Game.NATIVE_RESOLUTION.X/2)-(m_background.Width/2), (Game.NATIVE_RESOLUTION.Y / 2) - (m_background.Height / 2));
        m_cursor.Y = (int)(m_backgroundPosition.Y + BACKGROUND_MARGIN.Y);

        ReloadText();

        SetCursorPos();
    }

    private void ReloadText()
    {
        m_lines[(int)DialogResult.No] = m_textLoader.GetText("ModalDialog", "no");
        m_lines[(int)DialogResult.Yes] = m_textLoader.GetText("ModalDialog", "yes");
        m_lines[(int)DialogResult.Cancel] = m_textLoader.GetText("ModalDialog", "cancel");
        m_lines[(int)DialogResult.Confirm] = m_textLoader.GetText("ModalDialog", "confirm");

        for (int i = 0; i < m_lines.Length; i++)
        {
            Vector2 measurement = m_lineMeasurements[i] = m_font.MeasureString(m_lines[i]);
            m_lineOffsets[i] = (i == 0) ? 0 : m_lineOffsets[i - 1] + (int)measurement.X + OPTION_SPACING;
        }
    }

    private void SetCursorPos()
    {
        int position = m_cursorPos;
        m_cursor.X = (int)m_backgroundPosition.Y + BACKGROUND_MARGIN.X +  m_lineOffsets[position] + CURSOR_NUDGE_X;
        m_cursor.Width = (int)m_lineMeasurements[position].X + 32;
    }

    public override void Draw(GameTime gameTime)
    {
        Game.SpriteBatch.Draw(m_background, m_backgroundPosition, Color.White);

        for (int i = 0; i < m_optionIndexes.Length; i++)
        {
            string line = m_lines[i];
            Game.SpriteBatch.DrawString(m_font, line, m_backgroundPosition + new Vector2(BACKGROUND_MARGIN.X + m_lineOffsets[i], BACKGROUND_MARGIN.Y), Color.White);
        }
        
        base.Draw(gameTime);
    }
}