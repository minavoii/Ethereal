using HarmonyLib;

namespace Ethereal.Patches;

internal static class CombatState
{
    [HarmonyPatch(typeof(CombatController), "Initialize")]
    [HarmonyPostfix]
    private static void Postfix()
    {
        API.Summons.SetReady();
        API.Monsters.SetReady();
        API.LateReferenceables.Execute();
    }
}
