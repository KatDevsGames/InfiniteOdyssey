using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using InfiniteOdyssey.Behaviors;
using InfiniteOdyssey.Behaviors.Actors;
using InfiniteOdyssey.Extensions;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using MonoGame.Extended;

namespace InfiniteOdyssey.Scenes;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]//todo remove when done
public class ActionScene : Scene
{
    private TiledMap m_tileMap;
    private TiledMapRenderer m_tileMapRenderer;
    private Point m_tilemapSize;

    private List<TiledMap> m_adjacentTiledMaps = new();
    private List<TiledMapRenderer> m_adjacentTiledMapRenderers = new();

    private readonly HashSet<string> m_loadedTilemaps = new();

    private CollisionComponent m_collisionComponent;

    private SpriteBatch m_spriteBatch;

    private bool m_isLoaded;

    private Camera m_camera;

    private const float CAMERA_MOVE_SPEED = 1f;

    public ActionScene(Game game, bool active = true) : base(game, active) { }

    public override void Initialize()
    {
        AddBehavior("Camera", m_camera = new Camera(Game));
        m_spriteBatch = new SpriteBatch(Game.GraphicsDevice);

        Game.InputMapper.Action.Move += OnMove;
        Game.InputMapper.Action.CameraNudge += OnCameraNudge;
    }

    private void AddActor(CreatureBase actor)
    {
        AddBehavior(actor);
        foreach (HitBoxSegment segment in actor.HitBoxes) m_collisionComponent.Insert(segment);
    }

    private void RemoveActor(CreatureBase actor)
    {
        foreach (HitBoxSegment segment in actor.HitBoxes) m_collisionComponent.Remove(segment);
        RemoveBehavior(actor);
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
        m_isLoaded = true;
        Game.InputMapper.Mode = InputMapper.MapperMode.Action;
        TiledMap tileMap = m_tileMap = Game.Content.Load<TiledMap>("Maps\\Overworld\\Test");
        tileMap.GetVisibleLayersByType("Variation").ToArray();
        m_loadedTilemaps.Add("Maps\\Overworld\\Test");
        m_tileMapRenderer = new TiledMapRenderer(Game.GraphicsDevice, tileMap);
        Point tilemapSize = m_tilemapSize = new Point(tileMap.WidthInPixels, tileMap.HeightInPixels);
        m_camera.Bounds = new Vector2(tileMap.WidthInPixels - Game.NATIVE_RESOLUTION.X, tileMap.HeightInPixels - Game.NATIVE_RESOLUTION.Y);
        m_collisionComponent = new CollisionComponent(new RectangleF(Point2.Zero, tilemapSize));
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
        m_tileMapRenderer.Update(gameTime);
        base.Update(gameTime);
        m_collisionComponent.Update(gameTime);
    }
    public override void Draw(GameTime gameTime)
    {
        m_tileMapRenderer.Draw(m_camera.GetViewMatrix());
        base.Update(gameTime);
    }
}