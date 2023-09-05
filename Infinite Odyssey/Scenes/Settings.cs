using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Behaviors;
using InfiniteOdyssey.Loaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Scenes;

public class Settings : Scene
{
    //private readonly Scene m_modalReturn;

    private readonly InfiniteOdyssey.Settings m_settings;

    private SpriteFont m_font;

    private PinchCursor m_cursor;

    private int m_cursorPos = 0;

    private TextLoader m_textLoader;
    private readonly List<TextLoader.Locale> m_locales = TextLoader.Manifest.Locales.Values.ToList();
    private int m_selectedLocaleIndex;
    private TextLoader.Locale m_selectedLocale;
    private string? m_selectedLocaleName;

#if DESKTOP
    private readonly string[] m_lines = new string[9];
    private readonly Vector2[] m_lineMeasurements = new Vector2[9];
#else
    private readonly string[] m_lines = new string[8];
    private readonly Vector2[] m_lineMeasurements = new Vector2[8];
#endif

    private const int LINE_SPACING = 50;

    private const int CURSOR_NUDGE_Y = -8;

    private const float VOLUME_STEP = 0.05f;

    private const float TEXT_SPEED_STEP = 0.05f;

    private enum Selections
    {
        NoFlashing = 0,
        NoColors = 1,
        MusicVolume = 2,
        SFXVolume = 3,
        DialogVolume = 4,
        TextSpeed = 5,
        LanguageLocale = 6,
        InputMap = 7,
#if DESKTOP
        Display = 8
#endif
    }

    public Settings(Game game, bool active = true) : base(game, active)
    {
        m_textLoader = TextLoader.Instance;
        m_settings = game.Settings.Clone();

        m_selectedLocaleIndex = m_locales.FindIndex(l => string.Equals(l.Code, game.Settings.LanguageLocale));
        if (m_selectedLocaleIndex == -1) m_selectedLocaleIndex = m_locales.FindIndex(l => string.Equals(l.Code, TextLoader.DEFAULT_LOCALE));
        if (m_selectedLocaleIndex == -1) m_selectedLocaleIndex = 0;
        LoadLocale();

        game.InputMapper.Menu.Up += OnMenuUpDown;
        game.InputMapper.Menu.Down += OnMenuUpDown;
        game.InputMapper.Menu.Left += OnMenuLeftRight;
        game.InputMapper.Menu.Right += OnMenuLeftRight;
        game.InputMapper.Menu.Confirm += OnMenuConfirm;
        game.InputMapper.Menu.Cancel += OnMenuCancel;
    }

    private void OnMenuUpDown(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!Active) return;
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

    private void OnMenuLeftRight(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!Active) return;
        if (!e.Pressed) return;
        switch (e.EventType)
        {
            case InputMapper.MenuEvents.EventTypes.Left:
                CycleValue((Selections)m_cursorPos, true);
                return;
            case InputMapper.MenuEvents.EventTypes.Right:
                CycleValue((Selections)m_cursorPos, false);
                return;
        }
    }

    private void CycleValue(Selections selection, bool reverse)
    {
        switch (selection)
        {
            case Selections.MusicVolume:
                ChangeNormalized(ref m_settings.MusicVolume, reverse ? -VOLUME_STEP : VOLUME_STEP);
                break;
            case Selections.SFXVolume:
                ChangeNormalized(ref m_settings.SFXVolume, reverse ? -VOLUME_STEP : VOLUME_STEP);
                break;
            case Selections.DialogVolume:
                ChangeNormalized(ref m_settings.DialogVolume, reverse ? -VOLUME_STEP : VOLUME_STEP);
                break;
            case Selections.TextSpeed:
                ChangeNormalized(ref m_settings.DialogVolume, reverse ? -TEXT_SPEED_STEP : TEXT_SPEED_STEP);
                break;
            case Selections.LanguageLocale:
                CycleValue(ref m_selectedLocaleIndex, reverse ? -1 : 1, m_locales.Count);
                LoadLocale();
                ReloadText();
                break;
        }
    }

    private void LoadLocale()
    {
        TextLoader.Locale locale = m_locales[m_selectedLocaleIndex];
        m_selectedLocale = locale;
        m_selectedLocaleName = locale.LocalNames.TryGetValue(locale.Code, out string? localeName) ? localeName : locale.Name;
        m_textLoader = new(locale.Code);

    }

    private void CycleValue(ref int value, int change, int max)
    {
        value += change;
        if (value < 0) value = max - 1;
        else if (value >= max) value = 0;
    }

    private void ChangeNormalized(ref float value, float step)
    {
        value += step;
        if (value < 0) value = 0;
        if (value > 1) value = 1;
    }

    private void OnMenuConfirm(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!Active) return;
        if (!e.Pressed) return;
        switch ((Selections)m_cursorPos)
        {
            case Selections.NoFlashing:
                m_settings.NoFlashing = !m_settings.NoFlashing;
                break;
            case Selections.NoColors:
                m_settings.NoColors = !m_settings.NoColors;
                break;
            case Selections.InputMap:
                break;
#if DESKTOP
            case Selections.Display:
#endif
                break;
        }
    }

    private void OnMenuCancel(InputMapper.ButtonEventArgs<InputMapper.MenuEvents.EventTypes> e)
    {
        if (!Active) return;
        if (!e.Pressed) return;
        Game.SceneManager.Unload();
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
        AddBehavior("Cursor", m_cursor = new PinchCursor(Game) { X = (300 - 24), Width = 100 });
    }

    public override void LoadContent()
    {
        base.LoadContent();
        //Sound sound = CoreSystem.LoadStreamedSound("Music\\Title.ogg");
        //Channel channel = sound.Play();
        //channel.Looping = true;

        m_font = Game.Content.Load<SpriteFont>("Fonts\\SettingsMenu");

        ReloadText();

        SetCursorPos();
    }

    private void ReloadText()
    {

        m_lines[(int)Selections.NoFlashing] = m_textLoader.GetText("SettingsMenu", "noFlashing");
        m_lines[(int)Selections.NoColors] = m_textLoader.GetText("SettingsMenu", "noColors");
        m_lines[(int)Selections.MusicVolume] = m_textLoader.GetText("SettingsMenu", "musicVolume");
        m_lines[(int)Selections.SFXVolume] = m_textLoader.GetText("SettingsMenu", "sfxVolume");
        m_lines[(int)Selections.DialogVolume] = m_textLoader.GetText("SettingsMenu", "dialogVolume");
        m_lines[(int)Selections.TextSpeed] = m_textLoader.GetText("SettingsMenu", "textSpeed");
        m_lines[(int)Selections.LanguageLocale] = m_textLoader.GetText("SettingsMenu", "languageLocale");
        m_lines[(int)Selections.InputMap] = m_textLoader.GetText("SettingsMenu", "inputMap");
#if DESKTOP
        m_lines[(int)Selections.Display] = m_textLoader.GetText("SettingsMenu", "display");
#endif

        for (int i = 0; i < m_lines.Length; i++)
            m_lineMeasurements[i] = m_font.MeasureString(m_lines[i]);
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
            Game.SpriteBatch.DrawString(m_font, line, new Vector2(300, 100 + (LINE_SPACING * i)), Color.Black);
        }

        Game.SpriteBatch.DrawString(m_font, m_selectedLocaleName, new Vector2(500, 100 + (LINE_SPACING * (int)Selections.LanguageLocale)), Color.Black);
    }
}