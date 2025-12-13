using System.Threading.Tasks;

namespace Ethereal.Classes.API;

/// <summary>
/// A basic API.
/// </summary>
internal class BaseAPI
{
    /// <summary>
    /// `true` if the API is ready to be used, `false` otherwise.
    /// </summary>
    internal bool IsReady { get; private set; } = false;

    internal TaskCompletionSource<bool> TaskSource { get; private set; } = new();

    /// <summary>
    /// Mark the API as ready.
    /// </summary>
    internal virtual void SetReady()
    {
        IsReady = true;
        TaskSource.TrySetResult(true);
    }

    internal async Task<bool> WhenReady() => await TaskSource.Task;
}
