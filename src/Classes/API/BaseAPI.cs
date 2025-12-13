using System.Threading.Tasks;

namespace Ethereal.Classes.API;

/// <summary>
/// A basic API.
/// </summary>
internal class BaseAPI
{
    private readonly TaskCompletionSource<bool> ReadyTask = new();

    /// <summary>
    /// `true` if the API is ready to be used, `false` otherwise.
    /// </summary>
    internal bool IsReady => ReadyTask.Task.IsCompleted;

    /// <summary>
    /// Mark the API as ready.
    /// </summary>
    internal virtual void SetReady() => ReadyTask.TrySetResult(true);

    /// <summary>
    /// Wait until the API is ready.
    /// </summary>
    /// <returns></returns>
    internal async Task<bool> WhenReady() => await ReadyTask.Task;
}
