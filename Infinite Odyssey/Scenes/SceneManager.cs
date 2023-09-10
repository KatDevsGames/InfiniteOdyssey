using System.Collections.Generic;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Scenes;

public class SceneManager
{
    private readonly Dictionary<string, Scene> m_scenes = new();

    private Scene? m_activeScene;
    private readonly LinkedList<Scene> m_inactiveScenes = new();

    public void Add(Scene scene, string name) => m_scenes.Add(name, scene);

    public Scene Get(string sceneName) => m_scenes[sceneName];

    /*public void Load(Scene scene, string name)
    {
        Add(scene, name);
        Load(name);
    }*/

    public void Load(Scene scene)
    {
        scene.LoadContent();
        if (m_activeScene != null)
        {
            m_activeScene.Active = false;
            m_inactiveScenes.AddFirst(m_activeScene);
        }
        m_activeScene = scene;
        scene.Active = true;
    }

    public void Load(string sceneName)
    {
        Scene scene = m_scenes[sceneName];
        scene.LoadContent();
        if (m_activeScene != null)
        {
            m_activeScene.Active = false;
            m_inactiveScenes.AddFirst(m_activeScene);
        }
        m_activeScene = scene;
        scene.Active = true;
    }

    public void Return(object? value)
    {
        Unload();
        m_activeScene?.ReturnCallback(value);
    }

    public void Unload()
    {
        if (m_activeScene != null)
        {
            m_activeScene.Active = false;
            m_activeScene.UnloadContent();
        }
        if (m_inactiveScenes.TryPopFirst(out m_activeScene))
        {
            m_activeScene.Active = true;
        }
    }

    public void Unload(Scene scene)
    {
        if (!m_scenes.TryGetKey(scene, out string? sceneName)) return;
        Unload(sceneName);
    }

    public void Unload(string sceneName)
    {
        if (!m_scenes.Remove(sceneName, out Scene? scene)) return;
        if (ReferenceEquals(m_activeScene, scene))
        {
            m_activeScene.Active = false;
            m_activeScene.UnloadContent();
            if (m_inactiveScenes.TryPopFirst(out m_activeScene))
            {
                m_activeScene.Active = true;
            }
            return;
        }
        if (m_inactiveScenes.TryRemove(scene))
        {
            scene.Active = false;
            scene.UnloadContent();
        }
    }

    public void Initialize()
    {
        foreach (Scene scene in m_scenes.Values)
        {
            scene.Initialize();
        }
    }

    public void Update(GameTime gameTime)
    {
        m_activeScene?.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        LinkedListNode<Scene>? node = m_inactiveScenes.Last;
        while (node != null)
        {
            //if (!node.Value.Active) continue;
            node.Value.Draw(gameTime);
            node = node.Previous;
        }
        m_activeScene?.Draw(gameTime);
    }
}