using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;

namespace InfiniteOdyssey.Behaviors;

public class Camera : SceneBehavior
{
    private readonly OrthographicCamera m_camera;

    private Vector2 m_position;
    private Vector2 m_nudge;
    public Vector2 Bounds { get; set; }

    public Vector2 Position
    {
        get => m_position;
        set
        {
            CameraBound(ref value);
            m_position = value;
        }
    }

    public Camera(Game game) : base(game)
    {
        DefaultViewportAdapter viewportadapter = new(Game.GraphicsDevice);
        m_camera = new OrthographicCamera(viewportadapter);
    }

    private const int CAMERA_NUDGE_RANGE = 3 * 32;

    private const float CAMERA_NUDGE_SPEED = 1f;

    public void Nudge(Vector2 axisValue)
    {
        m_nudge = Vector2.Lerp(m_nudge, axisValue * CAMERA_NUDGE_RANGE, CAMERA_NUDGE_SPEED);
    }

    private void UpdateCameraPos()
    {
        Vector2 position = m_position + m_nudge;
        CameraBound(ref position);
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