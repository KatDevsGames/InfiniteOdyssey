using System;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Extensions;

[Flags]
public enum Direction4
{
    North = 1,
    South = 2,
    East = 4,
    West = 8
}

[Flags]
public enum Direction8
{
    North = 1,
    South = 2,
    East = 4,
    West = 8,
    Northeast = North & East,
    Northwest = North & West,
    Southeast = South & East,
    Southwest = South & West
}

public static class DirectionEx
{
    public static Direction4 GetDirection4(this Vector2 vector)
    {
        vector.Normalize();
        float x = vector.X;
        float y = vector.Y;
        float absX = MathF.Abs(x);
        float absY = MathF.Abs(y);
        if (absX > absY) return (x > 0) ? Direction4.East : Direction4.West;
        return (y > 0) ? Direction4.North : Direction4.South;
    }

    public static Point GetPoint(this Direction4 direction) =>
        direction switch
        {
            Direction4.North => new Point(0, -1),
            Direction4.South => new Point(0, 1),
            Direction4.East => new Point(1, 0),
            Direction4.West => new Point(-1, 0),
        };

    public static Direction4 GetOpposite(this Direction4 direction) =>
        direction switch
        {
            Direction4.North => Direction4.South,
            Direction4.South => Direction4.North,
            Direction4.East => Direction4.West,
            Direction4.West => Direction4.East
        };
}