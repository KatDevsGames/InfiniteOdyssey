using System;
using System.Runtime.CompilerServices;
using InfiniteOdyssey.Behaviors.Actors;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace InfiniteOdyssey.Behaviors.Camera;

public class Camera : SceneBehavior
{
    private readonly Player m_player;
    private readonly OrthographicCamera m_camera;

    private Vector2 m_position;
    private Vector2 m_nudge;

    private IActor m_follow;
    public IActor Follow
    {
        get => m_follow;
        set
        {
            value ??= m_player;
            m_follow = value;
            m_halfActor = new Vector2(value.Size.Width / 2f, value.Size.Height / 2f);
        }
    }

    private Vector2 m_halfActor;

    public Vector2 Bounds;

    public CameraMode Mode = CameraMode.Bounded | CameraMode.FollowActor | CameraMode.AllowNudge;

    public Vector2 Position
    {
        get => m_position;
        set
        {
            if (Mode.HasFlag(CameraMode.Bounded)) CameraBound(ref value);
            m_position = value;
        }
    }

    private static readonly Vector2 HALF_SCREEN = new (Game.NATIVE_RESOLUTION.X / 2f, Game.NATIVE_RESOLUTION.Y / 2f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector2 GetFollowCoordinates() => (Follow.Position + m_halfActor) - HALF_SCREEN;

    public Camera(Game game, Player player) : base(game)
    {
        Follow = m_player = player;
        DefaultViewportAdapter viewportadapter = new(Game.GraphicsDevice);
        m_camera = new OrthographicCamera(viewportadapter);
    }

    private const int CAMERA_NUDGE_RANGE = 3;
    private const float CAMERA_NUDGE_SPEED = 1f;

    private static readonly Vector2 CAMERA_NUDGE = new(CAMERA_NUDGE_RANGE * Game.TILE_SIZE.X, CAMERA_NUDGE_RANGE * Game.TILE_SIZE.Y);

    private const float CAMERA_FOLLOW_SPEED = 0.1f;

    public void Nudge(Vector2 axisValue)
    {
        m_nudge = Vector2.Lerp(m_nudge, axisValue * CAMERA_NUDGE, CAMERA_NUDGE_SPEED);
    }

    private void UpdateCameraPos()
    {
        Vector2 position = m_position;
        if (Mode.HasFlag(CameraMode.FollowActor))
        {
            Vector2 fc = GetFollowCoordinates();
            float amt = MathF.Max(1f, MathF.Abs(position.Length() - fc.Length()) * CAMERA_FOLLOW_SPEED);
            m_position = position = Vector2.Lerp(position, fc, amt);
        }
        if (Mode.HasFlag(CameraMode.AllowNudge)) position += m_nudge;
        if (Mode.HasFlag(CameraMode.Bounded)) CameraBound(ref position);
        m_camera.Position = position;
    }

    private void CameraBound(ref Vector2 position)
    {
        float maxX = Bounds.X;
        float maxY = Bounds.Y;

        if (position.X < 0) position.X = 0;
        else if (position.X > maxX) position.X = maxX;
        if (position.Y < 0) position.Y = 0;
        else if (position.Y > maxY) position.Y = maxY;
    }

    public override void Update(GameTime gameTime) => UpdateCameraPos();

    public Matrix GetViewMatrix() => m_camera.GetViewMatrix();
}