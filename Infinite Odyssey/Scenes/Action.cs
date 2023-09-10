using System.Collections.Generic;
using InfiniteOdyssey.Behaviors;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteOdyssey.Scenes;

public class Action : Scene
{
    private TiledMap m_tiledMap;
    private TiledMapRenderer m_tiledMapRenderer;

    private List<TiledMap> m_adjacentTiledMaps = new();
    private List<TiledMapRenderer> m_adjacentTiledMapRenderers = new();
    private Point m_tilemapSize;

    private SpriteBatch m_spriteBatch;

    private bool m_isLoaded;

    private Camera m_camera;

    private const float CAMERA_MOVE_SPEED = 1f;

    public Action(Game game, bool active = true) : base(game, active) { }

    public override void Initialize()
    {
        AddBehavior("Camera", m_camera = new(Game));
        m_spriteBatch = new(Game.GraphicsDevice);

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

    public HashSet<string> m_loadedTilemaps = new();

    public override void LoadContent()
    {
        m_isLoaded = true;
        Game.InputMapper.Mode = InputMapper.MapperMode.Action;
        TiledMap tiledMap = m_tiledMap = Game.Content.Load<TiledMap>("Maps\\Overworld\\Test");
        m_tiledMapRenderer = new(Game.GraphicsDevice, tiledMap);
        m_tilemapSize = new Point(tiledMap.WidthInPixels, tiledMap.HeightInPixels);
        m_camera.Bounds = new Vector2(tiledMap.WidthInPixels - Game.NATIVE_RESOLUTION.X, tiledMap.HeightInPixels - Game.NATIVE_RESOLUTION.Y);
    }

    public override void UnloadContent()
    {
        foreach (string tilemap in m_loadedTilemaps)
            Game.Content.UnloadAsset(tilemap);
        m_isLoaded = false;
    }

    public override void ReturnCallback(object? value)
    {
        Game.InputMapper.Mode = InputMapper.MapperMode.Action;
    }

    public override void Update(GameTime gameTime)
    {
        m_tiledMapRenderer.Update(gameTime);
        base.Update(gameTime);
    }
    public override void Draw(GameTime gameTime)
    {
        m_tiledMapRenderer.Draw(m_camera.GetViewMatrix());
        base.Update(gameTime);
    }
}