using System;
using System.Collections.Generic;
using System.Linq;
using InfiniteOdyssey.Extensions;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace InfiniteOdyssey.Randomization;

[Serializable]
public class RoomTemplate
{
    [JsonProperty(PropertyName = "name")]
    public string Name;

    [JsonProperty(PropertyName = "biome")]
    public Biome Biome;

    [JsonProperty(PropertyName = "level")]
    public int Level;

    [JsonIgnore]
    public Point Size;

    /// <summary>
    /// The name of the tilemap that this cell uses.
    /// </summary>
    [JsonProperty(PropertyName = "tileMap")]
    public string TileMap;

    [JsonProperty(PropertyName = "transitions")]
    public Dictionary<string, TransitionTemplate> Transitions;

    [JsonProperty(PropertyName = "transitionSeals")]
    public Dictionary<string, TransitionSeal> TransitionSeals;

    [JsonProperty(PropertyName = "variations")]
    public Dictionary<string, Variation> Variations;

    [JsonProperty(PropertyName = "enemies")]
    public Dictionary<string, EnemyTemplate> Enemies;

    [JsonProperty(PropertyName = "enemies")]
    public Dictionary<string, TreasureTemplate> Treasure;

    public IEnumerable<TransitionTemplate> GetTransitions(Rectangle bounds)
    {
        foreach (TransitionTemplate transition in Transitions.Values)
            if (transition.Bounds.Intersects(bounds)) yield return transition;
    }

    public enum ExitMatch
    {
        Error = -1,
        None = 0,
        Partial,
        Full
    }


    public ExitMatch IsExitMatch(Room other) => IsExitMatch(other.Location, other.Template);
    public ExitMatch IsExitMatch(Point location, RoomTemplate other)
    {
        Rectangle localRect = new(Point.Zero, Size);
        Rectangle otherRect = new(location, other.Size);

        Direction4 localDirection;
        if (localRect.Top == otherRect.Bottom) localDirection = Direction4.North;
        else if (localRect.Bottom == otherRect.Top) localDirection = Direction4.South;
        else if (localRect.Right == otherRect.Left) localDirection = Direction4.East;
        else if (localRect.Left == otherRect.Right) localDirection = Direction4.West;
        else return ExitMatch.Error;
        Direction4 remoteDirection = localDirection.GetOpposite();

        (Rectangle sourcePerimeter, Rectangle otherPerimeter) = localRect.SharedPerimeter(otherRect);

        if (sourcePerimeter.Size == Point.Zero) return ExitMatch.Error;

        int localTotal = 0;
        int localUnsealable = 0;
        foreach (TransitionTemplate transition in GetTransitions(sourcePerimeter))
        {
            if (transition.ExitType != ExitType.Standard) continue;
            if (transition.Direction != localDirection) continue;
            if (TransitionSeals.Values.All(s => s.Transition != transition.Name)) localUnsealable++;
            localTotal++;
        }

        int remoteTotal = 0;
        int remoteUnsealable = 0;
        foreach (TransitionTemplate transition in GetTransitions(otherPerimeter))
        {
            if (transition.ExitType != ExitType.Standard) continue;
            if (transition.Direction != remoteDirection) continue;
            if (TransitionSeals.Values.All(s => s.Transition != transition.Name)) remoteUnsealable++;
            remoteTotal++;
        }

        if (localTotal == remoteTotal) return ExitMatch.Full;
        if (localUnsealable == remoteUnsealable) return ExitMatch.Partial;
        return ExitMatch.None;
    }
}