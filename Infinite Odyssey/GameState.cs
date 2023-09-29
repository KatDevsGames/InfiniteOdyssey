using System;
using InfiniteOdyssey.Randomization;
using Newtonsoft.Json;

namespace InfiniteOdyssey;

[Serializable]
public class GameState
{
    [JsonProperty(PropertyName = "parameters")]
    public WorldParameters Parameters;

    [JsonProperty(PropertyName = "world")]
    public World World;

    [JsonProperty(PropertyName = "playerState")]
    public PlayerState PlayerState;

    public GameState() { }

    [JsonConstructor]
    public GameState(WorldParameters parameters, World world, PlayerState playerState)
    {
        Parameters = parameters;
        World = world;
        PlayerState = playerState;
    }

    public void Populate(Game game, WorldParameters parameters)
    {
        Parameters = parameters;
        WorldGenerator gen = new(game, parameters);
        World = gen.Generate();
        PlayerState = new PlayerState();
    }

    public void Populate(Game game, long seed, WorldParameters parameters)
    {
        Parameters = parameters;
        WorldGenerator gen = new(game, seed, parameters);
        World = gen.Generate();
        PlayerState = new PlayerState();
    }

    public void Populate(WorldParameters parameters, World world, PlayerState playerState)
    {
        Parameters = parameters;
        World = world;
        PlayerState = playerState;
    }
}