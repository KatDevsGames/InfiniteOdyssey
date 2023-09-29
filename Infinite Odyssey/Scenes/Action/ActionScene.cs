using System.Diagnostics.CodeAnalysis;
using InfiniteOdyssey.Behaviors.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InfiniteOdyssey.Behaviors.Camera;

namespace InfiniteOdyssey.Scenes.Action;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]//todo remove when done
public class ActionScene : Scene
{
    private readonly RoomContainer m_rooms;

    private SpriteBatch m_spriteBatch;

    private readonly Camera m_camera;

    private readonly Player m_player;

    private const float CAMERA_MOVE_SPEED = 1f;

    public ActionScene(Game game, bool active = true) : base(game, active)
    {
        Player player = m_player = new Player(this);
        Camera camera = m_camera = new Camera(game, player);
        m_rooms = new RoomContainer(game, player, camera);
    }

    public override void Initialize()
    {
        AddBehavior("Camera", m_camera);
        m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

        Game.InputMapper.Action.Move += OnMove;
        Game.InputMapper.Action.CameraNudge += OnCameraNudge;
    }

    private void OnMove(InputMapper.DirectionEventArgs<InputMapper.ActionEvents.EventTypes> e)
    {
        m_camera.Position += e.AxisValue * CAMERA_MOVE_SPEED;
    }

    private void OnCameraNudge(InputMapper.DirectionEventArgs<InputMapper.ActionEvents.EventTypes> e)
    {
        m_camera.Nudge(e.AxisValue);
    }

    public override void LoadContent()
    {
        Game.InputMapper.Mode = InputMapper.MapperMode.Action;

    }

    public override void UnloadContent()
    {

    }

    public override void ReturnCallback(object? value)
    {
        Game.InputMapper.Mode = InputMapper.MapperMode.Action;
    }

    public override void Update(GameTime gameTime)
    {
        m_rooms.Update(gameTime);
        base.Update(gameTime);
    }
    public override void Draw(GameTime gameTime)
    {
        m_rooms.Draw(gameTime);
        base.Draw(gameTime);
    }
}