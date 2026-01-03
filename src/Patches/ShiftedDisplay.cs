using System.Collections.Generic;
using Ethereal.API;
using Ethereal.Classes.Builders;
using HarmonyLib;
using UnityEngine;

namespace Ethereal.Patches;

internal static class ShiftedDisplay
{
    [HarmonyPatch(typeof(ShiftedMonsterDisplay), "InitializeShiftSprite")]
    [HarmonyPrefix]
    private static bool Prefix(Monster ___monster, ShiftedMonsterDisplay.ESpriteType ___spriteType, ref Dictionary<string, Sprite> ___Sprites)
    {
        Debug.Log($"InitializeShiftSprite");
        Debug.Log($"For {___spriteType}");

        // For some reason the CustomMonsterSprites don't carry over on clones
        Monsters.TryGet(___monster.ID, out Monster baseMonster);

        if (baseMonster.gameObject.TryGetComponent(out CustomMonsterSprites sprite))
        {
            Debug.Log($"Initializing shifted sprite for custom monster {___monster.Name}");

            ___Sprites = new Dictionary<string, Sprite>();
            Sprite[]? original = null;
            Sprite[]? shifted = null;
            switch(___spriteType)
            {
                case ShiftedMonsterDisplay.ESpriteType.Battle:
                    original = sprite.Sprites?.BattleSprites;
                    shifted = sprite.Sprites?.ShiftedBattleSprites;
                    break;
                case ShiftedMonsterDisplay.ESpriteType.Overworld:
                    original = sprite.Sprites?.OverworldSprites;
                    shifted = sprite.Sprites?.ShiftedOverworldSprites;
                    break;
                case ShiftedMonsterDisplay.ESpriteType.Projectile:
                    original = sprite.Sprites?.ProjectileSprites;
                    shifted = sprite.Sprites?.ShiftedProjectileSprites;
                    break;    
            }
            if (original == null || original.Length == 0 || shifted == null || shifted.Length == 0)
            {
                return false;
            }

            for (int i = 0;i < original.Length;i++)
            {
                Debug.Log(original[i].name);
                ___Sprites.Add(original[i].name, shifted[i]);
            }

            return false;
        }
        else
        {
            return true;
        }
    }
}