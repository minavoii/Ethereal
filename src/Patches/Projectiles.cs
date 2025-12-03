using HarmonyLib;

namespace Ethereal.Patches;

internal static class Projectiles
{
    [HarmonyPatch(typeof(Projectile), "Initialize")]
    [HarmonyPrefix]
    private static void Prefix(Projectile __instance)
    {
        __instance.gameObject.SetActive(true);  // Custom projectiles are disabled initially to avoid calling updates
    }
}
