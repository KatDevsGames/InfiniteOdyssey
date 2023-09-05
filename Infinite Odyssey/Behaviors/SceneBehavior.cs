using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Behaviors;

public class SceneBehavior
{
    public Game Game { get; }

    public SceneBehavior(Game game) => Game = game;

    public virtual void Initialize() { }

    public virtual void LoadContent() { }

    public virtual void UnloadContent() { }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime) { }
}