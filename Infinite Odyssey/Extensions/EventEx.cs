using System;
using System.Runtime.CompilerServices;

namespace InfiniteOdyssey.Extensions;

public static class EventEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeFirst(this Delegate ev, Predicate<Delegate> match, params object?[]? parameters)
    {
        foreach (Delegate del in ev.GetInvocationList())
        {
            if (!match(del)) continue;
            del.DynamicInvoke(parameters);
            return;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeFirst(this Delegate ev, Func<Delegate, bool> match, params object?[]? parameters)
    {
        foreach (Delegate del in ev.GetInvocationList())
        {
            if (!match(del)) continue;
            del.DynamicInvoke(parameters);
            return;
        }
    }
}