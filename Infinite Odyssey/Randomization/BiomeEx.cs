#if DEBUG
using System;
using Microsoft.Xna.Framework;

namespace InfiniteOdyssey.Randomization;

internal static class BiomeEx
{
    public static Color GetDebugColor(this Biome biome) =>
        biome switch {
            Biome.Field => Color.LightGreen,
            Biome.Forest => Color.Green,
            Biome.Mountain => Color.SaddleBrown,
            Biome.Volcano => Color.Red,
            Biome.Lake => Color.Blue,
            Biome.Ocean => Color.Navy,
            Biome.Desert => Color.Yellow,
            Biome.Canyon => Color.OrangeRed,
            Biome.Swamp => Color.YellowGreen,
            Biome.Frozen => Color.LightBlue,
            Biome.Wasteland => Color.DarkOrange,
            Biome.Ruins => Color.Beige,
            Biome.Cavern => Color.SlateGray,
            Biome.Jungle => Color.DarkTurquoise,
            Biome.Sky => Color.Turquoise,
            Biome.Void => Color.Black,
            Biome.Start => Color.Gray,
            Biome.Chaos => Color.Violet,
            Biome.Gate => Color.DarkSlateGray,
            Biome.Bridge => Color.LightGray,
            Biome.Phlogiston => Color.White,
            _ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
        };
}
#endif