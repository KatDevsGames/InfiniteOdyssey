using System;
using System.Collections;
using System.Collections.Generic;
using InfiniteOdyssey.Behaviors;
using InfiniteOdyssey.Extensions;
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

    private readonly SortedDictionary<int, List<SceneBehaviorEntry>> m_behaviorsPriority = new(ReverseComparer<int>.Instance);
    private readonly Dictionary<string, SceneBehaviorEntry> m_behaviorsName = new();

    private readonly LinkedList<IEnumerator> m_coroutines = new();
    private readonly List<IEnumerator> m_coroutinesToRemove = new(COROUTINE_REMOVAL_PREALLOC);

    private const int COROUTINE_REMOVAL_PREALLOC = 128;

    public void StartCoroutine(IEnumerable coroutine)
    {
        m_coroutines.AddLast(coroutine.GetEnumerator());
    }

    public bool Active { get; set; }

    public Game Game { get; }
    
    protected Scene(Game game, bool active = true)
    {
        Game = game;
        Active = active;
    }

    public virtual void ReturnCallback(object? value) { }

    //singleton version of this?
    public void AddBehavior(SceneBehavior behavior) => AddBehavior(Guid.NewGuid().ToString("D"), 0, behavior);
    public void AddBehavior(int priority, SceneBehavior behavior) => AddBehavior(Guid.NewGuid().ToString("D"), priority, behavior);
    public void AddBehavior(string name, SceneBehavior behavior) => AddBehavior(name, 0, behavior);
    public void AddBehavior(string name, int priority, SceneBehavior behavior)
    {
        SceneBehaviorEntry entry = new() { priority = priority, behavior = behavior };

        behavior.Initialize();
        m_behaviorsName.Add(name, entry);
        if (!m_behaviorsPriority.TryGetValue(priority, out List<SceneBehaviorEntry>? behaviors))
            m_behaviorsPriority[priority] = behaviors = new List<SceneBehaviorEntry>();

        behaviors.Add(entry);
    }

    public void RemoveBehavior(SceneBehavior entry)
    {
        foreach (var behaviorEntry in m_behaviorsName)
        {
            if (!Equals(behaviorEntry.Value.behavior, entry)) continue;
            RemoveBehavior(behaviorEntry.Key);
            return;
        }
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
        //scene updates
        foreach (List<SceneBehaviorEntry> entries in m_behaviorsPriority.Values)
        {
            foreach (SceneBehaviorEntry entry in entries)
            {
                entry.behavior.Update(gameTime);
            }
        }
        
        //coroutine updates
        foreach (IEnumerator coroutine in m_coroutines)
        {
            if (!coroutine.MoveNext()) m_coroutinesToRemove.Add(coroutine);
        }
        foreach (IEnumerator toRemove in m_coroutinesToRemove) m_coroutines.Remove(toRemove);
        m_coroutinesToRemove.Clear();
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