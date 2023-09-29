using System.Collections.Generic;
using InfiniteOdyssey.Randomization;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace InfiniteOdyssey.Scenes.Action;

public class RoomData
{
    public readonly RoomTemplate RoomTemplate;

    public readonly TiledMap TiledMap;

    public readonly string TiledMapAssetName;

    public readonly TiledMapRenderer Renderer;

    public readonly Point Size;

    public Point Position;

    public readonly Vector2 CameraBounds;

    public readonly List<ICollisionActor> Actors = new();
    public CollisionComponent CollisionComponent { get; private set; }

    public RoomData(Game game, RoomTemplate roomTemplate, Point position)
    {
        RoomTemplate = roomTemplate;
        string tilemap = TiledMapAssetName = "Maps\\" + roomTemplate.TileMap;
        TiledMap tiledMap = TiledMap = game.Content.Load<TiledMap>(tilemap);
        Renderer = new TiledMapRenderer(game.GraphicsDevice, tiledMap);
        Point size = Size = new Point(tiledMap.WidthInPixels, tiledMap.HeightInPixels);
        CameraBounds = new Vector2(tiledMap.WidthInPixels - Game.NATIVE_RESOLUTION.X, tiledMap.HeightInPixels - Game.NATIVE_RESOLUTION.Y);
        CollisionComponent = new CollisionComponent(new RectangleF(position, size));
    }
}