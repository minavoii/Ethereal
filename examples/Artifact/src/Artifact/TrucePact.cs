using System.Collections.Generic;
using System.IO;

namespace Artifacts.Artifacts;

internal static class TrucePact
{
    internal const int Id = 7001;

    internal const string Name = "Truce Pact";

    internal static readonly Ethereal.API.Artifacts.ArtifactDescriptor descriptor = new()
    {
        id = Id,
        name = Name,
        targetType = ETargetType.SingleAlly,
        icon = Ethereal.API.Icon.LoadFromImage(
            Ethereal.API.Icon.IconType.Artifact,
            Path.Join(Plugin.EquipmentsPath, "Artifact_TrucePact.png")
        ),
        actionIconBig = Ethereal.API.Icon.LoadFromImage(
            Ethereal.API.Icon.IconType.Action,
            Path.Join(Plugin.EquipmentsPath, "ActionIcon_TrucePact_Big.png")
        ),
        actionIconSmall = Ethereal.API.Icon.LoadFromImage(
            Ethereal.API.Icon.IconType.ActionSmall,
            Path.Join(Plugin.EquipmentsPath, "ActionIcon_TrucePact_Small.png")
        ),
        actions =
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

    internal static readonly LocalisationData.LocalisationDataEntry localisationData = new()
    {
        ID = Id,
        StringContent = Name,
        StringContentEnglish = Name,
        StringContentFrench = "Pacte de Trêve",
    };

    internal static readonly Dictionary<string, string> customLanguageEntries = new()
    {
        // Assuming a custom language named `Newlang` exists
        { "Newlang", "Pact" },
    };
}
