using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace InfiniteOdyssey.Extensions;

public static class PointEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point2 Add(this Point2 p1, Point2 p2) => new(p1.X + p2.X, p1.Y + p2.Y);

    public static Vector2 AsVector2(this Point2 pt) => new(pt.X, pt.Y);
}