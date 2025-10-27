using BepInEx.Logging;

namespace Ethereal;

internal class Log
{
    internal static ManualLogSource Plugin { get; set; } = null!;

    internal static ManualLogSource API { get; } = Logger.CreateLogSource("Ethereal.API");
}
