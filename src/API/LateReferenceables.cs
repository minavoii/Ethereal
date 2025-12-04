using System;
using System.Collections.Generic;
using HarmonyLib;

namespace Ethereal.API;

internal static partial class LateReferenceables
{
    private static Queue<Action> _actions = new Queue<Action>();

    /// <summary>
    /// Enqueues a late task for hooking up Referenceables
    /// </summary>
    internal static void Queue(Action action)
    {
        _actions.AddItem(action);
    }

    internal static void Execute()
    {
        while (_actions.Count > 0)
        {
            Action action = _actions.Dequeue();
            action();
        }
    }
}