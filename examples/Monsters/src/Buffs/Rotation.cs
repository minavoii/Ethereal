using System.IO;
using Ethereal.API;
using UnityEngine;

namespace ExampleMonsters.CustomBuffs;

internal static class Rotation
{
    internal static readonly AnimationClip Animation = Animations.LoadFromAsset(
        Path.Join(Plugin.CustomMonstersPath, "Rotation"),
        "assets/animations/waterwheel/rotation.prefab"
    );

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
        IsDamagingBuff = false,
        VFXApply = VFXs.CreateCosmetic(Animation),
        SfxApply = new(),
        PassiveEffectList = [],
    };
}
