using System;
using System.Collections.Concurrent;

/// <summary>
/// A basic API.
/// </summary>
internal class BaseAPI
{
    /// <summary>
    /// `true` if the API is ready to be used, `false` otherwise.
    /// </summary>
    internal bool IsReady { get; private set; } = false;

    /// <summary>
    /// Mark the API as ready.
    /// </summary>
    internal virtual void SetReady() => IsReady = true;
}

/// <summary>
/// An API that contains a queue for use when the API isn't yet ready.
/// </summary>
internal class QueueableAPI : BaseAPI
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
