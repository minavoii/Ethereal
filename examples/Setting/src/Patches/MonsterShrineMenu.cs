using System.Collections.Generic;
using System.Linq;
using Ethereal.Classes.Settings;
using HarmonyLib;

namespace ExampleSetting.Patches;

internal static class MonsterShrineMenuPatch
{
    [HarmonyPatch(typeof(MonsterShrineMenu), "GetMonstersFromMementos")]
    [HarmonyPrefix]
    static bool GetMonstersFromMementos(MonsterShrineMenu __instance, ref List<Monster> __result)
    {
        if (!GameSettingsController.Instance.GetCustom<bool>("Build Tester Mode"))
        {
            return true;
        }

        __instance.MonsterShrineTrigger.GenerateMementosForShrine(ignoreHasData: true);
        __result = __instance.MonsterShrineTrigger.ShrineSpecificSouls.Select(mon => MonsterManager.Instance.GetMonster(mon.Monster.ID)).ToList();

        return false;
    }
}