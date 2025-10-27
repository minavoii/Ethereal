using System.IO;
using Ethereal.API;
using UnityEngine;

namespace ExampleMonsters.CustomBuffs;

internal static class Rotation
{
    private static readonly Sprite Icon = Sprites.LoadFromImage(
        Sprites.SpriteType.Buff,
        Path.Join(Plugin.CustomMonstersPath, "Status_Rotation.png")
    );

    internal static readonly Buff Buff = new()
    {
        ID = 1900,
        Name = "Rotation",
        Description = "No innate effect. Rotation traits increase in strength every turn.",
        BuffType = EBuffType.Buff,
        Icon = Icon,
        MonsterHUDIcon = Icon,
        MonsterHUDIconSmall = Icon,
        IsAfflictionDebuff = false,
        IsCrowdControl = false,
        IsDamagingBuff = false,
        VFXApply = new(),
        SfxApply = new(),
        PassiveEffectList = [],
    };
}
