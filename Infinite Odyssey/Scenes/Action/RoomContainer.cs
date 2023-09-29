using System.Collections.Generic;
using InfiniteOdyssey.Behaviors.Actors;
using InfiniteOdyssey.Behaviors.Camera;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Scenes.Action;

public class RoomContainer : Dictionary<string, RoomData>
{
    private readonly Game m_game;

    private readonly Player m_player;

    private readonly Camera m_camera;

    private RoomData m_activeRoom;

    public RoomData ActiveRoom { get; private set; }

    public RoomContainer(Game game, Player player, Camera camera)
    {
        m_game = game;
        m_player = player;
        m_camera = camera;
    }

    public void LoadRoom(string name)
    {
        RoomData room = this[name];
        m_activeRoom = room;
        m_camera.Bounds = room.CameraBounds;
    }

    public void UnloadAll()
    {
        foreach (RoomData room in Values)
            m_game.Content.UnloadAsset(room.TiledMapAssetName);
    }

    public void Update(GameTime gameTime)
    {
        //foreach (RoomData room in Values)
        //    room.Renderer.Update(gameTime);
        m_activeRoom.Renderer.Update(gameTime);
        m_activeRoom.CollisionComponent.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        //foreach (RoomData room in Values)
        //    room.Renderer.Draw(m_camera.GetViewMatrix());
        m_activeRoom.Renderer.Draw(m_camera.GetViewMatrix());
    }
}