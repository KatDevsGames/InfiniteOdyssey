using InfiniteOdyssey.Randomization;

namespace InfiniteOdyssey;

public class GameState
{
    public WorldParameters Parameters;

    public World World;

    public object Player;

    public void Populate(WorldParameters parameters)
    {
        Parameters = parameters;
        WorldGenerator gen = new(parameters);
        World = gen.Generate();
        Player = new object();
    }

    public void Populate(long seed, WorldParameters parameters)
    {
        Parameters = parameters;
        WorldGenerator gen = new(seed, parameters);
        World = gen.Generate();
        Player = new object();
    }

    public void Populate(WorldParameters parameters, World world, object player)
    {
        Parameters = parameters;
        World = world;
        Player = player;
    }
}