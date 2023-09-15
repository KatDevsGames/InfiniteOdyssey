using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Behaviors;
using InfiniteOdyssey.Extensions;
using InfiniteOdyssey.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Scenes;

public class ModalDialogScene : MenuBase
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

    private DialogType m_type;
    private DialogResult m_cancelValue;

    private string m_prompt;
    private Vector2 m_promptMeasurement;
    private Vector2 m_promptOffset;

    private string[] m_lines;
    private Vector2[] m_lineMeasurements;
    private int m_lineMargin;
    private int[] m_lineOffsetsX;
    private int m_lineOffsetY;

    private const int PROMPT_MARGIN_Y = 16;

    private static readonly Point CURSOR_NUDGE = new(-24, -8);

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
        YesNoCancel = 1,
        Confirm = 2,
        ConfirmCancel = 3
    }

    private static readonly string[] OPTION_NAMES = { "yes", "no", "cancel", "confirm" };

    private static readonly int[][] OPTION_INDEXES = {
        new []{ (int)DialogResult.Yes, (int)DialogResult.No },
        new []{ (int)DialogResult.Yes, (int)DialogResult.No, (int)DialogResult.Cancel  },
        new []{ (int)DialogResult.Confirm },
        new []{ (int)DialogResult.Confirm, (int)DialogResult.Cancel },
    };

    private int[] m_optionIndexes;

    public ModalDialogScene(Game game, bool active = true) : base(game, active)
    {
        m_textLoader = TextLoader.Instance;
        
        game.InputMapper.Menu.Left += OnMenuLeftRight;
        game.InputMapper.Menu.Right += OnMenuLeftRight;
        game.InputMapper.Menu.Confirm += OnMenuConfirm;
        game.InputMapper.Menu.Cancel += OnMenuCancel;
    }

    public void SetType(string prompt, DialogType type, DialogResult cancelValue)
    {
        m_prompt = prompt;
        m_type = type;
        m_optionIndexes = OPTION_INDEXES[(int)type];
        m_cancelValue = cancelValue;
        m_cursorPos = m_optionIndexes.IndexOf((int)cancelValue);

        int len = m_optionIndexes.Length;
        m_lines = new string[len];
        m_lineMeasurements = new Vector2[len];
        m_lineOffsetsX = new int[len];
    }

    private void OnMenuLeftRight(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!e.Pressed) return;
        switch (e.EventType)
        {
            case InputMapper.MenuEvents.EventTypes.Left:
                CycleValue(ref m_cursorPos, -1, m_optionIndexes.Length);
                break;
            case InputMapper.MenuEvents.EventTypes.Right:
                CycleValue(ref m_cursorPos, 1, m_optionIndexes.Length);
                break;
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

        ReloadText();
        SetCursorPos();
    }

    private void ReloadText()
    {
        m_promptMeasurement = m_font.MeasureString(m_prompt);
        m_lineMargin = (int)m_promptMeasurement.Y + PROMPT_MARGIN_Y;

        int totalWidth = 32 * m_optionIndexes.Length;
        for (int i = 0; i < m_optionIndexes.Length; i++)
        {
            string line = m_lines[i] = m_textLoader.GetText("ModalDialog", OPTION_NAMES[m_optionIndexes[i]]);
            Vector2 measurement = m_lineMeasurements[i] = m_font.MeasureString(line);
            totalWidth += (int)measurement.X;
        }

        m_background = GetFrame(FrameColor.Red, (Math.Max((int)m_promptMeasurement.X, totalWidth) / 32) + 2, 5);
        m_backgroundPosition = new Vector2((Game.NATIVE_RESOLUTION.X / 2) - (m_background.Width / 2), (Game.NATIVE_RESOLUTION.Y / 2) - (m_background.Height / 2));
        
        m_promptOffset = m_backgroundPosition + new Vector2((m_background.Width / 2) - ((int)m_promptMeasurement.X / 2), BACKGROUND_MARGIN.Y);

        int offsetY = m_lineOffsetY = (int)m_backgroundPosition.Y + BACKGROUND_MARGIN.Y + m_lineMargin;
        m_cursor.Y = offsetY + (int)(m_lineMeasurements[0].Y / 2) + CURSOR_NUDGE.Y;
        
        for (int i = 0; i < m_optionIndexes.Length; i++)
        {
            m_lineOffsetsX[i] = (int)((m_backgroundPosition.X + (m_background.Width * ((i + 1) / (float)(m_optionIndexes.Length + 1)))) - (m_lineMeasurements[i].X / 2));
        }
    }

    private void SetCursorPos()
    {
        int position = m_cursorPos;
        m_cursor.X = m_lineOffsetsX[position] + CURSOR_NUDGE.X;
        m_cursor.Width = (int)m_lineMeasurements[position].X + 32;
    }

    public override void Draw(GameTime gameTime)
    {
        Game.SpriteBatch.Draw(m_background, m_backgroundPosition, Color.White);

        // ReSharper disable once PossibleLossOfFraction
        Game.SpriteBatch.DrawString(m_font, m_prompt, m_promptOffset, Color.White);

        for (int i = 0; i < m_optionIndexes.Length; i++)
        {
            string line = m_lines[i];
            Game.SpriteBatch.DrawString(m_font, line, new Vector2(m_lineOffsetsX[i], m_lineOffsetY), Color.White);
        }
        
        base.Draw(gameTime);
    }
}