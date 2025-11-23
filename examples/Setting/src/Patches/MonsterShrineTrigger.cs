using Ethereal.Classes.Settings;
using HarmonyLib;

namespace ExampleSetting.Patches;

internal static class MonsterShrineTriggerPatch
{
    [HarmonyPatch(typeof(MonsterShrineTrigger), "GenerateMementosForShrine")]
    [HarmonyPrefix]
    static bool GenerateMementosForShrine(MonsterShrineTrigger __instance)
    {
        if (!GameSettingsController.Instance.GetCustom<bool>("Build Tester Mode"))
        {
            return true;
        }

        __instance.ShrineSpecificSouls.Clear();
        foreach (var monster in InventoryManager.Instance.GetAvailableMonsterSouls(excludeActiveMonsters: true))
        {
            MonsterMemento memento = new MonsterMemento
            {
                Monster = monster
            };
            __instance.ShrineSpecificSouls.Add(memento);
        }

        return false;
    }
}