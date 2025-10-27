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

    /// <summary>
    /// Mark the API as ready.
    /// </summary>
    // internal virtual void SetReady() => IsReady = true;

    internal virtual void SetReady()
    {
        IsReady = true;
        TaskSource.TrySetResult(true);
    }

    internal TaskCompletionSource<bool> TaskSource { get; } = new();
}
