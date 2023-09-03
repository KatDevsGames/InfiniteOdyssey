using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Scenes;

public class SceneManager
{
    private readonly Dictionary<string, Scene> m_scenes = new();

    private Scene? m_activeScene;

    public void Add(Scene scene, string name) => m_scenes.Add(name, scene);

    public Scene Get(string sceneName) => m_scenes[sceneName];

    public void Load(string sceneName)
    {
        Scene scene = m_scenes[sceneName];
        Scene? wasActive = m_activeScene;
        m_activeScene = null;
        wasActive?.UnloadContent();
        scene.LoadContent();
        m_activeScene = scene;
    }

    public void Initalize()
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
        m_activeScene?.Draw(gameTime);
    }
}