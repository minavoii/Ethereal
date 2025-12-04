using Ethereal.CustomFlags;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.Patches;

internal static class MonsterInit
{
    /// <summary>
    /// Required for custom monsters.
    /// Custom monsters are initially disabled (to avoid rendering their Sprite)
    /// However on instantiating them, Awake needs to be called, so we set them active
    /// prior to instantiation.
    /// </summary>
    [HarmonyPatch(typeof(MonsterManager), "AddMonsterByPrefab")]
    [HarmonyPrefix]
    private static void Prefix(MonsterManager __instance, GameObject monsterPrefab)
    {
        if (monsterPrefab.IsCustomObject())
        {
            monsterPrefab.SetActive(true);
        }
    }

    /// <summary>
    /// Required for custom monsters.
    /// Custom monsters are initially disabled (to avoid rendering their Sprite)
    /// After instantiation, we should set them back to disabled.
    /// </summary>
    /// <param name="action"></param>
    [HarmonyPatch(typeof(MonsterManager), "AddMonsterByPrefab")]
    [HarmonyPostfix]
    private static void Postfix(MonsterManager __instance, GameObject monsterPrefab)
    {
        if (monsterPrefab.IsCustomObject())
        {
            monsterPrefab.SetActive(false);
        }
    }
}
