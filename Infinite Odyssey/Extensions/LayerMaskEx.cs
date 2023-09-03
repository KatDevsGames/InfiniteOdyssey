namespace InfiniteOdyssey.Extensions;

public static class LayerMaskEx
{
    public enum LayerMask
    {
        Ground = 1 << 8,
        HiddenGround = 1 << 9,
        FalseGround = 1 << 10,
        PhaseGround = 1 << 11,

        Platform = 1 << 13,
        CreaturePlatform = 1 << 14,

        Default = 1 << 0,
        TransparentFX = 1 << 1,
        IgnoreRaycast = 1 << 2,
        Water = 1 << 4,
        UI = 1 << 5
    }
}