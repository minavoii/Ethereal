using System.Collections.Generic;
using System.IO;
using Ethereal.API;

namespace ExampleArtifacts.Artifact;

internal static class TrucePact
{
    internal const int Id = 7001;

    internal const string Name = "Truce Pact";

    internal static readonly Artifacts.ArtifactDescriptor Descriptor = new()
    {
        Id = Id,
        Name = Name,
        TargetType = ETargetType.SingleAlly,
        Icon = Sprites.LoadFromImage(
            Sprites.SpriteType.Artifact,
            Path.Join(Plugin.ArtifactsPath, "Artifact_TrucePact.png")
        ),
        ActionIconBig = Sprites.LoadFromImage(
            Sprites.SpriteType.Action,
            Path.Join(Plugin.ArtifactsPath, "ActionIcon_TrucePact_Big.png")
        ),
        ActionIconSmall = Sprites.LoadFromImage(
            Sprites.SpriteType.ActionSmall,
            Path.Join(Plugin.ArtifactsPath, "ActionIcon_TrucePact_Small.png")
        ),
        Actions =
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
        ],
    };

    internal static readonly LocalisationData.LocalisationDataEntry LocalisationData = new()
    {
        ID = Id,
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
