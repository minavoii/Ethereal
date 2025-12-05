using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.API;

internal static partial class LateReferenceables
{
    private static Queue<Action> _actions = new Queue<Action>();

    /// <summary>
    /// Enqueues a late task for hooking up Referenceables
    /// </summary>
    internal static void Queue(Action action)
    {
        Debug.Log("Queueing late reference action");
        _actions.Enqueue(action);
    }

    internal static void Execute()
    {
        Debug.Log($"Executing {_actions.Count} late reference actions");
        while (_actions.Count > 0)
        {
            Action action = _actions.Dequeue();
            action();
        }
    }
}