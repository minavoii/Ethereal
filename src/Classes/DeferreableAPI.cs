using System;
using System.Collections.Concurrent;

/// <summary>
/// An API that contains a queue for use when the API isn't yet ready.
/// </summary>
internal class DeferreableAPI : BaseAPI
{
    private readonly ConcurrentQueue<Action> Queue = new();

    /// <summary>
    /// Run all methods in the queue.
    /// </summary>
    private void RunQueue()
    {
        while (Queue.TryDequeue(out Action method))
            method();
    }

    /// <summary>
    /// Add a new method to the queue.
    /// </summary>
    /// <param name="method"></param>
    internal void Enqueue(Action method)
    {
        Queue.Enqueue(method);
    }

    /// <summary>
    /// Mark the API as ready and run all methods in the queue.
    /// </summary>
    internal override void SetReady()
    {
        base.SetReady();
        RunQueue();
    }
}
