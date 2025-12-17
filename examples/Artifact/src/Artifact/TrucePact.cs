using System.Collections.Generic;
using System.IO;
using Ethereal.API;
using Ethereal.Classes.Builders;

namespace ExampleArtifacts.Artifact;

internal static class TrucePact
{
    private const int ID = 7001;

    private const string Name = "Truce Pact";

    internal static readonly ArtifactBuilder Builder = new(
        ID: ID,
        Name: Name,
        TargetType: ETargetType.SingleAlly,
        Icon: Sprites.LoadFromImage(Path.Join(Plugin.ArtifactsPath, "Artifact_TrucePact.png")),
        IconBig: Sprites.LoadFromImage(
            Path.Join(Plugin.ArtifactsPath, "ActionIcon_TrucePact_Big.png")
        ),
        IconSmall: Sprites.LoadFromImage(
            Path.Join(Plugin.ArtifactsPath, "ActionIcon_TrucePact_Small.png")
        ),
        Modifiers:
        [
            new ActionHeal()
            {
                HealAmount = 15,
                HealCount = 2,
                Condition = ActionHeal.EHealCondition.None,
            },
            new ActionShield()
            {
                ShieldAmount = 10,
                ShieldCount = 4,
                ShieldType = ActionShield.EShieldType.Normal,
                OverrideTarget = true,
                Target = ETargetType.AllEnemies,
            },
        ]
    );

    internal static readonly LocalisationData.LocalisationDataEntry LocalisationData = new()
    {
        ID = ID,
        StringContent = Name,
        StringContentEnglish = Name,
        StringContentFrench = "Pacte de Trêve",
    };

    internal static readonly Dictionary<string, string> CustomLanguageEntries = new()
    {
        // Assuming a custom language named `Newlang` exists
        { "Newlang", "Pact" },
    };
}
