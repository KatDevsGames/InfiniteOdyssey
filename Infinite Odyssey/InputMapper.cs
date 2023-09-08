using System;
using System.Collections.Generic;
using FmodForFoxes;
using InfiniteOdyssey.Extensions;
using InfiniteOdyssey.Scenes;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json.Linq;

namespace InfiniteOdyssey;

public class InputMapper
{
    public Game Game { get; }

    private readonly KeyboardListener m_keyboardListener = new();

    private readonly GamePadListener m_gamePadListener = new();

    private static readonly Predicate<Delegate> CALLBACK_FILTER = d => (d.Target is not Scene s) || s is { Active: true };

    public enum MapperMode
    {
        Menu,
        Action
    }

    public MapperMode Mode { get; set; }

    public interface IInputEventArgs { }

    public struct DirectionEventArgs<T> : IInputEventArgs where T : Enum
    {
        public readonly T EventType;
        public readonly Vector2 AxisValue;

        public DirectionEventArgs(T eventType, Vector2 axisValue)
        {
            EventType = eventType;
            AxisValue = axisValue;
        }
    }

    public struct ButtonEventArgs<T> : IInputEventArgs where T : Enum
    {
        public readonly T EventType;
        public readonly bool Pressed;

        public ButtonEventArgs(T eventType, bool pressed)
        {
            EventType = eventType;
            Pressed = pressed;
        }
    }

    public MenuEvents Menu { get; } = new();
    public class MenuEvents
    {
        public enum EventTypes
        {
            Up, Down, Left, Right,
            Confirm, Cancel,
            Menu
        }

        public void FireEvent(EventTypes ev, IInputEventArgs e)
        {
            switch (ev)
            {
                case EventTypes.Up:
                    Up?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Down:
                    Down?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Left:
                    Left?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Right:
                    Right?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Confirm:
                    Confirm?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Cancel:
                    Cancel?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Menu:
                    Menu?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
            }
        }

        public void ImportMap(JObject map)
        {
            m_gamepadMap.Clear();
            m_gamepadMap = map["gamepadMap"].Value<Dictionary<EventTypes, Buttons>>();

            m_keyboardMap.Clear();
            m_keyboardMap = map["keyboardMap"].Value<Dictionary<EventTypes, Keys>>();

            //SwapSticks = map["swapSticks"].Value<bool>();
        }

        public void SetDefaults()
        {
            m_gamepadMap.Clear();
            m_gamepadMap.Add(EventTypes.Confirm, Buttons.A);
            m_gamepadMap.Add(EventTypes.Cancel, Buttons.B);

            m_gamepadMap.Add(EventTypes.Menu, Buttons.Start);

            m_keyboardMap.Clear();
            m_keyboardMap.Add(EventTypes.Up, Keys.W);
            m_keyboardMap.Add(EventTypes.Down, Keys.S);
            m_keyboardMap.Add(EventTypes.Left, Keys.A);
            m_keyboardMap.Add(EventTypes.Right, Keys.D);

            m_keyboardMap.Add(EventTypes.Confirm, Keys.K);
            m_keyboardMap.Add(EventTypes.Cancel, Keys.L);

            m_keyboardMap.Add(EventTypes.Menu, Keys.Escape);
        }

        public JObject ExportMap() =>
            new()
            {
                { "gamepadMap", JObject.FromObject(GamepadMap) },
                { "keyboardMap", JObject.FromObject(KeyboardMap) }
            };

        private Dictionary<EventTypes, Buttons> m_gamepadMap = new();
        public IReadOnlyDictionary<EventTypes, Buttons> GamepadMap => m_gamepadMap;

        private Dictionary<EventTypes, Keys> m_keyboardMap = new();
        public IReadOnlyDictionary<EventTypes, Keys> KeyboardMap => m_keyboardMap;

        //public bool SwapSticks { get; set; }

        public event Action<ButtonEventArgs<EventTypes>>? Up;
        public event Action<ButtonEventArgs<EventTypes>>? Down;
        public event Action<ButtonEventArgs<EventTypes>>? Left;
        public event Action<ButtonEventArgs<EventTypes>>? Right;

        public event Action<ButtonEventArgs<EventTypes>>? Confirm;
        public event Action<ButtonEventArgs<EventTypes>>? Cancel;

        public event Action<ButtonEventArgs<EventTypes>>? Menu;
    }

    public ActionEvents Action { get; } = new();
    public class ActionEvents
    {
        public enum EventTypes
        {
            MoveUp, MoveDown, MoveLeft, MoveRight,
            CameraNudgeUp, CameraNudgeDown, CameraNudgeLeft, CameraNudgeRight,
            Move, CameraNudge,
            Confirm, Cancel,
            Menu, Map,
            Attack, Item, Run,
            ItemCycleLeft, ItemCycleRight
        }

        public void FireEvent(EventTypes ev, IInputEventArgs e)
        {
            switch (ev)
            {
                case EventTypes.Move:
                case EventTypes.MoveUp:
                case EventTypes.MoveDown:
                case EventTypes.MoveLeft:
                case EventTypes.MoveRight:
                    Move?.InvokeWhere(CALLBACK_FILTER, (DirectionEventArgs<EventTypes>)e);
                    break;
                case EventTypes.CameraNudge:
                    CameraNudge?.InvokeWhere(CALLBACK_FILTER, (DirectionEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Confirm:
                    Confirm?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Cancel:
                    Cancel?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Menu:
                    Menu?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Map:
                    Map?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Attack:
                    Attack?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Item:
                    Item?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.Run:
                    Run?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.ItemCycleLeft:
                    ItemCycleLeft?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
                case EventTypes.ItemCycleRight:
                    ItemCycleRight?.InvokeWhere(CALLBACK_FILTER, (ButtonEventArgs<EventTypes>)e);
                    break;
            }
        }

        public void ImportMap(JObject map)
        {
            m_gamepadMap.Clear();
            m_gamepadMap = map["gamepadMap"].Value<Dictionary<EventTypes, Buttons>>();

            m_keyboardMap.Clear();
            m_keyboardMap = map["keyboardMap"].Value<Dictionary<EventTypes, Keys>>();

            DPad = map["dPad"].Value<EventTypes>();

            SwapSticks = map["swapSticks"].Value<bool>();
        }

        public void SetDefaults()
        {
            m_gamepadMap.Clear();
            m_gamepadMap.Add(EventTypes.Confirm, Buttons.A);
            m_gamepadMap.Add(EventTypes.Cancel, Buttons.B);

            m_gamepadMap.Add(EventTypes.Menu, Buttons.Start);
            m_gamepadMap.Add(EventTypes.Map, Buttons.Y);

            m_gamepadMap.Add(EventTypes.Attack, Buttons.A);
            m_gamepadMap.Add(EventTypes.Item, Buttons.X);
            m_gamepadMap.Add(EventTypes.Run, Buttons.B);

            m_gamepadMap.Add(EventTypes.ItemCycleLeft, Buttons.LeftTrigger);
            m_gamepadMap.Add(EventTypes.ItemCycleRight, Buttons.RightTrigger);

            m_keyboardMap.Clear();
            m_keyboardMap.Add(EventTypes.MoveUp, Keys.W);
            m_keyboardMap.Add(EventTypes.MoveDown, Keys.S);
            m_keyboardMap.Add(EventTypes.MoveLeft, Keys.A);
            m_keyboardMap.Add(EventTypes.MoveRight, Keys.D);

            m_keyboardMap.Add(EventTypes.CameraNudgeUp, Keys.Up);
            m_keyboardMap.Add(EventTypes.CameraNudgeDown, Keys.Down);
            m_keyboardMap.Add(EventTypes.CameraNudgeLeft, Keys.Left);
            m_keyboardMap.Add(EventTypes.CameraNudgeRight, Keys.Right);

            m_keyboardMap.Add(EventTypes.Confirm, Keys.K);
            m_keyboardMap.Add(EventTypes.Cancel, Keys.L);

            m_keyboardMap.Add(EventTypes.Menu, Keys.Escape);
            m_keyboardMap.Add(EventTypes.Map, Keys.M);

            m_keyboardMap.Add(EventTypes.Attack, Keys.K);
            m_keyboardMap.Add(EventTypes.Item, Keys.O);
            m_keyboardMap.Add(EventTypes.Run, Keys.L);

            m_keyboardMap.Add(EventTypes.ItemCycleLeft, Keys.OemMinus);
            m_keyboardMap.Add(EventTypes.ItemCycleRight, Keys.OemPlus);

            DPad = EventTypes.Move;
            SwapSticks = false;
        }

        public JObject ExportMap() =>
            new()
            {
                { "gamepadMap", JObject.FromObject(GamepadMap) },
                { "keyboardMap", JObject.FromObject(KeyboardMap) },
                { "dPad", JToken.FromObject(DPad) },
                { "swapSticks", SwapSticks }
            };

        private Dictionary<EventTypes, Buttons> m_gamepadMap = new();
        public IReadOnlyDictionary<EventTypes, Buttons> GamepadMap => m_gamepadMap;

        private Dictionary<EventTypes, Keys> m_keyboardMap = new();
        public IReadOnlyDictionary<EventTypes, Keys> KeyboardMap => m_keyboardMap;

        public EventTypes DPad { get; set; }

        public bool SwapSticks { get; set; }

        public event Action<DirectionEventArgs<EventTypes>>? Move;
        public event Action<DirectionEventArgs<EventTypes>>? CameraNudge;

        public event Action<ButtonEventArgs<EventTypes>>? Confirm;
        public event Action<ButtonEventArgs<EventTypes>>? Cancel;
        
        public event Action<ButtonEventArgs<EventTypes>>? Menu;
        public event Action<ButtonEventArgs<EventTypes>>? Map;
        
        public event Action<ButtonEventArgs<EventTypes>>? Attack;
        public event Action<ButtonEventArgs<EventTypes>>? Item;
        public event Action<ButtonEventArgs<EventTypes>>? Run;
        
        public event Action<ButtonEventArgs<EventTypes>>? ItemCycleLeft;
        public event Action<ButtonEventArgs<EventTypes>>? ItemCycleRight;
    }

    public InputMapper(Game game)
    {
        Game = game;
        m_gamePadListener.ButtonDown += OnGamePadButton;
        m_gamePadListener.ButtonUp += OnGamePadButton;
        m_gamePadListener.ThumbStickMoved += OnGamePadStick;

        m_keyboardListener.KeyPressed += OnKeyboardButtonDown;
        m_keyboardListener.KeyReleased += OnKeyboardButtonUp;

        ImportSettings();
    }

    public void ImportSettings()
    {
        JObject? settings = Game.Settings.InputMap;
        if (settings != null)
        {
            Menu.ImportMap((JObject)settings["action"]);
            Action.ImportMap((JObject)settings["action"]);
            return;
        }
        Menu.SetDefaults();
        Action.SetDefaults();
    }

    public void ExportSettings() => Game.Settings.InputMap = new()
    {
        {"menu", Menu.ExportMap()},
        {"action", Action.ExportMap()}
    };

    private void OnKeyboardButtonUp(object? sender, KeyboardEventArgs e) => OnKeyboardButton(e, false);
    private void OnKeyboardButtonDown(object? sender, KeyboardEventArgs e) => OnKeyboardButton(e, true);
    private Vector2 m_keyboardMoveDirection = Vector2.Zero;
    private Vector2 m_keyboardCameraNudgeDirection = Vector2.Zero;

    private void OnKeyboardButton(KeyboardEventArgs e, bool keyDown)
    {
        switch (Mode)
        {
            case MapperMode.Menu:
                OnKeyboardButtonMenu(e, keyDown);
                return;
            case MapperMode.Action:
                OnKeyboardButtonAction(e, keyDown);
                return;
        }
    }

    private void OnKeyboardButtonMenu(KeyboardEventArgs e, bool keyDown)
    {
        MenuEvents.EventTypes ev;
        switch (e.Key)
        {
            case Keys.Up:
                ev = MenuEvents.EventTypes.Up;
                break;
            case Keys.Down:
                ev = MenuEvents.EventTypes.Down;
                break;
            case Keys.Left:
                ev = MenuEvents.EventTypes.Left;
                break;
            case Keys.Right:
                ev = MenuEvents.EventTypes.Right;
                break;
            case Keys.Enter:
                ev = MenuEvents.EventTypes.Confirm;
                break;
            case Keys.Back:
                ev = MenuEvents.EventTypes.Cancel;
                break;
            case Keys.Escape:
                ev = MenuEvents.EventTypes.Menu;
                break;
            default:
                if (!Menu.KeyboardMap.TryGetKey(e.Key, out ev)) return;
                Menu.FireEvent(ev, new ButtonEventArgs<MenuEvents.EventTypes>(ev, keyDown));
                return;
        }
        Menu.FireEvent(ev, new ButtonEventArgs<MenuEvents.EventTypes>(ev, keyDown));
    }

    private void OnKeyboardButtonAction(KeyboardEventArgs e, bool keyDown)
    {
        if (!Action.KeyboardMap.TryGetKey(e.Key, out ActionEvents.EventTypes ev)) return;

        Vector2 direction;
        switch (ev)
        {
            case ActionEvents.EventTypes.MoveUp:
                m_keyboardMoveDirection.Y += keyDown ? -1 : 1;
                m_keyboardMoveDirection.Y.Truncate(1);
                direction = m_keyboardMoveDirection;
                break;
            case ActionEvents.EventTypes.MoveDown:
                m_keyboardMoveDirection.Y += keyDown ? 1 : -1;
                m_keyboardMoveDirection.Y.Truncate(1);
                direction = m_keyboardMoveDirection;
                break;
            case ActionEvents.EventTypes.MoveLeft:
                m_keyboardMoveDirection.X += keyDown ? -1 : 1;
                m_keyboardMoveDirection.X.Truncate(1);
                direction = m_keyboardMoveDirection;
                break;
            case ActionEvents.EventTypes.MoveRight:
                m_keyboardMoveDirection.X += keyDown ? 1 : -1;
                m_keyboardMoveDirection.X.Truncate(1);
                direction = m_keyboardMoveDirection;
                break;
            case ActionEvents.EventTypes.CameraNudgeUp:
                m_keyboardCameraNudgeDirection.Y += keyDown ? -1 : 1;
                m_keyboardCameraNudgeDirection.Y.Truncate(1);
                direction = m_keyboardCameraNudgeDirection;
                break;
            case ActionEvents.EventTypes.CameraNudgeDown:
                m_keyboardCameraNudgeDirection.Y += keyDown ? 1 : -1;
                m_keyboardCameraNudgeDirection.Y.Truncate(1);
                direction = m_keyboardCameraNudgeDirection;
                break;
            case ActionEvents.EventTypes.CameraNudgeLeft:
                m_keyboardCameraNudgeDirection.X += keyDown ? -1 : 1;
                m_keyboardCameraNudgeDirection.X.Truncate(1);
                direction = m_keyboardCameraNudgeDirection;
                break;
            case ActionEvents.EventTypes.CameraNudgeRight:
                m_keyboardCameraNudgeDirection.X += keyDown ? 1 : -1;
                m_keyboardCameraNudgeDirection.X.Truncate(1);
                direction = m_keyboardCameraNudgeDirection;
                break;
            default:
                Action.FireEvent(ev, new ButtonEventArgs<ActionEvents.EventTypes>(ev, keyDown));
                return;
        }
        Action.FireEvent(ev, new DirectionEventArgs<ActionEvents.EventTypes>(ev, direction));
    }
    private void OnGamePadButton(object? sender, GamePadEventArgs e)
    {
        switch (Mode)
        {
            case MapperMode.Menu:
                OnGamePadButtonMenu(e);
                return;
            case MapperMode.Action:
                OnGamePadButtonAction(e);
                return;
        }
    }

    private void OnGamePadButtonMenu(GamePadEventArgs e)
    {
        MenuEvents.EventTypes ev;
        switch (e.Button)
        {
            case Buttons.DPadUp or Buttons.LeftThumbstickUp or Buttons.RightThumbstickUp:
                ev = MenuEvents.EventTypes.Up;
                break;
            case Buttons.DPadDown or Buttons.LeftThumbstickDown or Buttons.RightThumbstickDown:
                ev = MenuEvents.EventTypes.Down;
                break;
            case Buttons.DPadLeft or Buttons.LeftThumbstickLeft or Buttons.RightThumbstickLeft:
                ev = MenuEvents.EventTypes.Left;
                break;
            case Buttons.DPadRight or Buttons.LeftThumbstickRight or Buttons.RightThumbstickRight:
                ev = MenuEvents.EventTypes.Right;
                break;
            default:
                if (!Menu.GamepadMap.TryGetKey(e.Button, out ev)) return;
                Menu.FireEvent(ev, new ButtonEventArgs<MenuEvents.EventTypes>(ev, e.CurrentState.IsButtonDown(e.Button)));
                return;
        }
        Menu.FireEvent(ev, new ButtonEventArgs<MenuEvents.EventTypes>(ev, e.CurrentState.IsButtonDown(e.Button)));
    }

    private void OnGamePadButtonAction(GamePadEventArgs e)
    {
        if (!Action.GamepadMap.TryGetKey(e.Button, out ActionEvents.EventTypes ev)) return;
        // dpad can be bound to either move or nudge - kat
        if (ev is ActionEvents.EventTypes.Move or ActionEvents.EventTypes.CameraNudge)
        {
            Vector2 direction = Vector2.Zero;
            if ((e.CurrentState.DPad.Up == ButtonState.Pressed) && (e.CurrentState.DPad.Down == ButtonState.Released))
            {
                direction.Y = -1f;
            }
            else if ((e.CurrentState.DPad.Down == ButtonState.Pressed) && (e.CurrentState.DPad.Up == ButtonState.Released))
            {
                direction.Y = 1f;
            }
            if ((e.CurrentState.DPad.Left == ButtonState.Pressed) && (e.CurrentState.DPad.Right == ButtonState.Released))
            {
                direction.X = -1f;
            }
            else if ((e.CurrentState.DPad.Right == ButtonState.Pressed) && (e.CurrentState.DPad.Left == ButtonState.Released))
            {
                direction.X = 1f;
            }
            Action.FireEvent(ev, new DirectionEventArgs<ActionEvents.EventTypes>(ev, direction));
        }
        else
        {
            Action.FireEvent(ev, new ButtonEventArgs<ActionEvents.EventTypes>(ev, e.CurrentState.IsButtonDown(e.Button)));
        }
    }

    private void OnGamePadStick(object? sender, GamePadEventArgs e)
    {
        switch (Mode)
        {
            case MapperMode.Menu:
                // handled by button mapper
                return;
            case MapperMode.Action:
                OnGamePadStickAction(e);
                return;
        }
    }

    private void OnGamePadStickAction(GamePadEventArgs e)
    {
        ActionEvents.EventTypes ev;
        switch (e.Button)
        {
            case Buttons.LeftStick when (!Action.SwapSticks):
            case Buttons.RightStick when Action.SwapSticks:
                ev = ActionEvents.EventTypes.Move;
                break;
            case Buttons.RightStick when (!Action.SwapSticks):
            case Buttons.LeftStick when Action.SwapSticks:
                ev = ActionEvents.EventTypes.CameraNudge;
                break;
            default:
                return;
        }
        Action.FireEvent(ev, new DirectionEventArgs<ActionEvents.EventTypes>(ev, e.ThumbStickState));
    }

    public void Update(GameTime gameTime)
    {
        FmodManager.Update();
        m_keyboardListener.Update(gameTime);
        m_gamePadListener.Update(gameTime);
    }
}