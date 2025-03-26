using BepInEx.Logging;

namespace Ethereal;

internal class Log
{
    internal static ManualLogSource Plugin;

    internal static ManualLogSource API = Logger.CreateLogSource("Ethereal.API");
}
