using System;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Extensions;

public static class RectangleEx
{
    /// <remarks>
    /// If the boxes do not touch, the returned sizes will both be zero and the second returned
    /// perimeter will be at the wrong location.
    /// </remarks>
    public static (Rectangle sourcePerimeter, Rectangle otherPerimeter) SharedPerimeter(this Rectangle source, Rectangle other)
    {
        Rectangle intersection = Rectangle.Intersect(source, other);

        if (!intersection.IsEmpty) throw new ArgumentException("The supplied rectangles overlap.");

        // Determine which side they touch and create a border
        Rectangle sourcePerimeter;
        Rectangle otherPerimeter;

        if (source.Right == other.Left)
        {
            other.Offset(-1, 0);
            sourcePerimeter = Rectangle.Intersect(source, other);
            otherPerimeter = sourcePerimeter;
            otherPerimeter.Offset(1, 0);
        }
        else if (source.Left == other.Right)
        {
            other.Offset(1, 0);
            sourcePerimeter = Rectangle.Intersect(source, other);
            otherPerimeter = sourcePerimeter;
            otherPerimeter.Offset(-1, 0);
        }
        else if (source.Bottom == other.Top)
        {
            other.Offset(0, -1);
            sourcePerimeter = Rectangle.Intersect(source, other);
            otherPerimeter = sourcePerimeter;
            otherPerimeter.Offset(0, 1);
        }
        else if (source.Top == other.Bottom)
        {
            other.Offset(0, 1);
            sourcePerimeter = Rectangle.Intersect(source, other);
            otherPerimeter = sourcePerimeter;
            otherPerimeter.Offset(0, -1);
        }
        else throw new ArgumentException("The supplied rectangles are not adjacent.");

        return (sourcePerimeter, otherPerimeter);
    }
}