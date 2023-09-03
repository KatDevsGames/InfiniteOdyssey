using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Scenes;

public abstract class Scene
{
#pragma warning disable CS8618
    private class SceneBehaviorEntry
    {
        public int priority;
        public SceneBehavior behavior;
        public bool loaded;
    }
#pragma warning restore CS8618

    private readonly SortedDictionary<int, List<SceneBehaviorEntry>> m_behaviorsPriority = new();
    private readonly Dictionary<string, SceneBehaviorEntry> m_behaviorsName = new();

    protected Game Game { get; }

    // ReSharper disable once VirtualMemberCallInConstructor
    protected Scene(Game game) => Game = game;

    //singleton version of this?
    public void AddBehavior(string name, SceneBehavior behavior) => AddBehavior(name, 0, behavior);
    public void AddBehavior(string name, int priority, SceneBehavior behavior)
    {
        SceneBehaviorEntry entry = new() { priority = priority, behavior = behavior };

        behavior.Initialize();
        m_behaviorsName.Add(name, entry);
        if (!m_behaviorsPriority.TryGetValue(priority, out List<SceneBehaviorEntry>? behaviors))
            m_behaviorsPriority[priority] = behaviors = new();

        behaviors.Add(entry);
    }

    public void RemoveBehavior(string name)
    {
        SceneBehaviorEntry entry = m_behaviorsName[name];
        m_behaviorsName.Remove(name);
        m_behaviorsPriority[entry.priority].Remove(entry);
        if (entry.loaded)
        {
            entry.behavior.UnloadContent();
            entry.loaded = false;
        }
    }

    public virtual void Initialize() { }
    
    public virtual void LoadContent()
    {
        foreach (List<SceneBehaviorEntry> entries in m_behaviorsPriority.Values)
        {
            foreach (SceneBehaviorEntry entry in entries)
            {
                if (!entry.loaded)
                {
                    entry.behavior.LoadContent();
                    entry.loaded = true;
                }
            }
        }
    }
    
    public virtual void UnloadContent()
    {
        foreach (List<SceneBehaviorEntry> entries in m_behaviorsPriority.Values)
        {
            foreach (SceneBehaviorEntry entry in entries)
            {
                if (entry.loaded)
                {
                    entry.behavior.UnloadContent();
                    entry.loaded = false;
                }
            }
        }
    }
    
    public virtual void Update(GameTime gameTime)
    {
        foreach (List<SceneBehaviorEntry> entries in m_behaviorsPriority.Values)
        {
            foreach (SceneBehaviorEntry entry in entries)
            {
                entry.behavior.Update(gameTime);
            }
        }
    }
    
    public virtual void Draw(GameTime gameTime)
    {
        foreach (List<SceneBehaviorEntry> entries in m_behaviorsPriority.Values)
        {
            foreach (SceneBehaviorEntry entry in entries)
            {
                entry.behavior.Draw(gameTime);
            }
        }
    }
}