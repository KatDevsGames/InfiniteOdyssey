using System;
using System.Collections.Generic;
using FmodForFoxes;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input.InputListeners;
using Newtonsoft.Json.Linq;

namespace InfiniteOdyssey;

public class InputMapper
{
    private readonly KeyboardListener m_keyboardListener = new();

    private readonly GamePadListener m_gamePadListener = new();

    public interface IInputEventArgs { }

    public struct DirectionEventArgs : IInputEventArgs
    {
        public readonly Vector2 AxisValue;

        public DirectionEventArgs(Vector2 axisValue)
        {
            AxisValue = axisValue;
        }
    }

    public struct ButtonEventArgs : IInputEventArgs
    {
        public readonly bool Pressed;

        public ButtonEventArgs(bool pressed)
        {
            Pressed = pressed;
        }
    }

    public MenuEvents Menu { get; } = new();
    public class MenuEvents
    {
        private event Action<GamePadEventArgs>? Up;
        private event Action<GamePadEventArgs>? Down;
        private event Action<GamePadEventArgs>? Left;
        private event Action<GamePadEventArgs>? Right;

        private event Action<GamePadEventArgs>? Confirm;
        private event Action<GamePadEventArgs>? Cancel;
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
                    Move?.Invoke((DirectionEventArgs)e);
                    break;
                case EventTypes.CameraNudge:
                    CameraNudge?.Invoke((DirectionEventArgs)e);
                    break;
                case EventTypes.Confirm:
                    Confirm?.Invoke((ButtonEventArgs)e);
                    break;
                case EventTypes.Cancel:
                    Cancel?.Invoke((ButtonEventArgs)e);
                    break;
                case EventTypes.Menu:
                    Menu?.Invoke((ButtonEventArgs)e);
                    break;
                case EventTypes.Map:
                    Map?.Invoke((ButtonEventArgs)e);
                    break;
                case EventTypes.Attack:
                    Attack?.Invoke((ButtonEventArgs)e);
                    break;
                case EventTypes.Item:
                    Item?.Invoke((ButtonEventArgs)e);
                    break;
                case EventTypes.Run:
                    Run?.Invoke((ButtonEventArgs)e);
                    break;
                case EventTypes.ItemCycleLeft:
                    ItemCycleLeft?.Invoke((ButtonEventArgs)e);
                    break;
                case EventTypes.ItemCycleRight:
                    ItemCycleRight?.Invoke((ButtonEventArgs)e);
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

        public event Action<DirectionEventArgs>? Move;
        public event Action<DirectionEventArgs>? CameraNudge;

        public event Action<ButtonEventArgs>? Confirm;
        public event Action<ButtonEventArgs>? Cancel;
        
        public event Action<ButtonEventArgs>? Menu;
        public event Action<ButtonEventArgs>? Map;
        
        public event Action<ButtonEventArgs>? Attack;
        public event Action<ButtonEventArgs>? Item;
        public event Action<ButtonEventArgs>? Run;
        
        public event Action<ButtonEventArgs>? ItemCycleLeft;
        public event Action<ButtonEventArgs>? ItemCycleRight;
    }

    public InputMapper()
    {
        m_gamePadListener.ButtonDown += OnGamePadButton;
        m_gamePadListener.ButtonUp += OnGamePadButton;
        m_gamePadListener.ThumbStickMoved += OnGamePadStick;

        m_keyboardListener.KeyPressed += OnKeyboardButtonDown;
        m_keyboardListener.KeyReleased += OnKeyboardButtonUp;
    }

    public void ImportSettings()
    {
        JObject? settings = Settings.InputMap;
        if(settings != null)
        {
            Action.ImportMap((JObject)settings["action"]);
            return;
        }
        Action.SetDefaults();
    }

    public void ExportSettings() => Settings.InputMap = new() { { "action", Action.ExportMap() } };

    private void OnKeyboardButtonUp(object? sender, KeyboardEventArgs e) => OnKeyboardButton(e, false);
    private void OnKeyboardButtonDown(object? sender, KeyboardEventArgs e) => OnKeyboardButton(e, true);
    private Vector2 m_keyboardMoveDirection = Vector2.Zero;
    private Vector2 m_keyboardCameraNudgeDirection = Vector2.Zero;
    private void OnKeyboardButton(KeyboardEventArgs e, bool keyDown)
    {
        if (!Action.KeyboardMap.TryGetKey(e.Key, out ActionEvents.EventTypes ev)) return;

        switch (ev)
        {
            case ActionEvents.EventTypes.MoveUp:
                m_keyboardMoveDirection.Y += keyDown ? -1 : 1;
                m_keyboardMoveDirection.Y.Truncate(1);
                Action.FireEvent(ActionEvents.EventTypes.Move, new DirectionEventArgs(m_keyboardMoveDirection));
                break;
            case ActionEvents.EventTypes.MoveDown:
                m_keyboardMoveDirection.Y += keyDown ? 1 : -1;
                m_keyboardMoveDirection.Y.Truncate(1);
                Action.FireEvent(ActionEvents.EventTypes.Move, new DirectionEventArgs(m_keyboardMoveDirection));
                break;
            case ActionEvents.EventTypes.MoveLeft:
                m_keyboardMoveDirection.X += keyDown ? -1 : 1;
                m_keyboardMoveDirection.X.Truncate(1);
                Action.FireEvent(ActionEvents.EventTypes.Move, new DirectionEventArgs(m_keyboardMoveDirection));
                break;
            case ActionEvents.EventTypes.MoveRight:
                m_keyboardMoveDirection.X += keyDown ? 1 : -1;
                m_keyboardMoveDirection.X.Truncate(1);
                Action.FireEvent(ActionEvents.EventTypes.Move, new DirectionEventArgs(m_keyboardMoveDirection));
                break;
            case ActionEvents.EventTypes.CameraNudgeUp:
                m_keyboardCameraNudgeDirection.Y += keyDown ? -1 : 1;
                m_keyboardCameraNudgeDirection.Y.Truncate(1);
                Action.FireEvent(ActionEvents.EventTypes.Move, new DirectionEventArgs(m_keyboardCameraNudgeDirection));
                break;
            case ActionEvents.EventTypes.CameraNudgeDown:
                m_keyboardCameraNudgeDirection.Y += keyDown ? 1 : -1;
                m_keyboardCameraNudgeDirection.Y.Truncate(1);
                Action.FireEvent(ActionEvents.EventTypes.Move, new DirectionEventArgs(m_keyboardCameraNudgeDirection));
                break;
            case ActionEvents.EventTypes.CameraNudgeLeft:
                m_keyboardCameraNudgeDirection.X += keyDown ? -1 : 1;
                m_keyboardCameraNudgeDirection.X.Truncate(1);
                Action.FireEvent(ActionEvents.EventTypes.Move, new DirectionEventArgs(m_keyboardCameraNudgeDirection));
                break;
            case ActionEvents.EventTypes.CameraNudgeRight:
                m_keyboardCameraNudgeDirection.X += keyDown ? 1 : -1;
                m_keyboardCameraNudgeDirection.X.Truncate(1);
                Action.FireEvent(ActionEvents.EventTypes.Move, new DirectionEventArgs(m_keyboardCameraNudgeDirection));
                break;
            default:
                Action.FireEvent(ev, new ButtonEventArgs(keyDown));
                break;
        }
    }

    private void OnGamePadButton(object? sender, GamePadEventArgs e)
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
            Action.FireEvent(ev, new DirectionEventArgs(direction));
        }
        else
        {
            Action.FireEvent(ev, new ButtonEventArgs(e.CurrentState.IsButtonDown(e.Button)));
        }
    }

    private void OnGamePadStick(object? sender, GamePadEventArgs e)
    {
        switch (e.Button)
        {
            case Buttons.LeftStick when (!Action.SwapSticks):
            case Buttons.RightStick when Action.SwapSticks:
                Action.FireEvent(ActionEvents.EventTypes.Move, new DirectionEventArgs(e.ThumbStickState));
                break;
            case Buttons.RightStick when (!Action.SwapSticks):
            case Buttons.LeftStick when Action.SwapSticks:
                Action.FireEvent(ActionEvents.EventTypes.CameraNudge, new DirectionEventArgs(e.ThumbStickState));
                break;
        }
    }

    public void Update(GameTime gameTime)
    {
        FmodManager.Update();
        m_keyboardListener.Update(gameTime);
        m_gamePadListener.Update(gameTime);
    }
}