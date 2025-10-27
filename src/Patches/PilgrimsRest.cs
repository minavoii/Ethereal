using Ethereal.API;
using HarmonyLib;

namespace Ethereal.Patches;

internal static class PilgrimsRest
{
    [HarmonyPatch(typeof(PilgrimsRestScript), "Awake")]
    [HarmonyPrefix]
    private static void Prefix()
    {
        MetaUpgrades.SetReady();
    }
}
