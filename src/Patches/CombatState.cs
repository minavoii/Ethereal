using HarmonyLib;

namespace Ethereal.Patches;

internal static class CombatState
{
    [HarmonyPatch(typeof(CombatStateManager), "Awake")]
    [HarmonyPostfix]
    private static void Postfix()
    {
        API.Monsters.SetReady();
        API.Mementos.SetReady();
    }
}
