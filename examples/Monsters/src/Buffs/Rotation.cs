using System.IO;
using Ethereal.API;
using Ethereal.Classes.Builders;
using UnityEngine;

namespace ExampleMonsters.CustomBuffs;

internal static class Rotation
{
    private const int ID = 1900;

    private const string Name = "Rotation";

    private const string Description =
        "No innate effect. {Rotation} traits increase in strength every turn.";

    internal static readonly AnimationClip Animation = Animations.LoadFromBundle(
        Path.Join(Plugin.CustomMonstersPath, "Rotation"),
        "assets/animations/waterwheel/rotation.prefab"
    );

    private static readonly Sprite Icon = Sprites.LoadFromImage(
        Path.Join(Plugin.CustomMonstersPath, "Status_Rotation.png")
    );

    internal static readonly Buff Buff = new()
    {
        ID = ID,
        Name = Name,
        Description = Description,
        BuffType = EBuffType.Buff,
        Icon = Icon,
        MonsterHUDIcon = Icon,
        MonsterHUDIconSmall = Icon,
        IsAfflictionDebuff = false,
        IsDamagingBuff = false,
        VFXApply = null,
        SfxApply = new(),
        PassiveEffectList = [],
    };

    internal static readonly KeywordBuilder Keyword = new(
        Name: Name,
        Description: Description,
        Identifier: [Name],
        Color: new(0.6745098f, 0.8784314f, 0.6941177f, 1f)
    );
}
