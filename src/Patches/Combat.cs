using HarmonyLib;

namespace Ethereal.Patches;

internal static class Combat
{
    [HarmonyPatch(typeof(CombatController), "Initialize")]
    [HarmonyPostfix]
    private static void Postfix()
    {
        API.Monsters.SetReady();
    }
}
