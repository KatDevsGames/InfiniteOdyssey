using System;
using System.Runtime.CompilerServices;

namespace InfiniteOdyssey.Extensions;

public static class EventEx
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeWhere(this Delegate ev, Predicate<Delegate> match, params object?[]? parameters)
    {
        foreach (Delegate del in ev.GetInvocationList())
        {
            if (!match(del)) continue;
            del.DynamicInvoke(parameters);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvokeWhere(this Delegate ev, Func<Delegate, bool> match, params object?[]? parameters)
    {
        foreach (Delegate del in ev.GetInvocationList())
        {
            if (!match(del)) continue;
            del.DynamicInvoke(parameters);
        }
    }
}