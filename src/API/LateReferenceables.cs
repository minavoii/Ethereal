using System;
using Ethereal.Attributes;

namespace Ethereal.API;

[Deferrable]
public static partial class LateReferenceables
{
    /// <summary>
    /// Enqueues a late task for hooking up Referenceables
    /// </summary>
    [Deferrable]
    private static void Queue_Impl(Action action)
    {
        action();
    }
}