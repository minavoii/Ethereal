using System.Collections.Generic;
using System.Linq;
using Ethereal.API;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.Patches;

internal static class ShiftedDisplay
{
    [HarmonyPatch(typeof(ShiftedMonsterDisplay), "InitializeShiftSprite")]
    [HarmonyPrefix]
    private static bool Prefix(Monster ___monster, ref Dictionary<string, Sprite> ___Sprites)
    {
        if (Sprites.ShiftedSprites.TryGetValue(___monster.ID, out Sprite[] sprites))
        {
            ___Sprites = sprites.ToDictionary(x => x.name, x => x);
            return false;
        }

        return true;
    }
}
