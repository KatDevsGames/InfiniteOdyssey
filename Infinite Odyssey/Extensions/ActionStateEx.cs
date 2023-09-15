using InfiniteOdyssey.Controllers;

namespace InfiniteOdyssey.Extensions
{
    public static class ActionStateEx
    {
        public static bool IsPaused(this ActionController.ActionState state) => ((int)state) >= 0x100;
    }
}