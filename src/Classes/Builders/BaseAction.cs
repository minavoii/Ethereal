using System.Collections.Generic;
using System.Linq;
using Ethereal.API;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a BaseAction at runtime.
/// </summary>
/// <param name="ID"></param>
/// <param name="Name"></param>
/// <param name="Cost"></param>
/// <param name="ActionType"></param>
/// <param name="TargetType"></param>
/// <param name="SkillType"></param>
/// <param name="Types"></param>
/// <param name="Elements"></param>
/// <param name="AnimationType"></param>
/// <param name="ActionIconBig"></param>
/// <param name="ActionIconSmall"></param>
/// <param name="ActionIconCutSmall"></param>
/// <param name="FreeAction"></param>
/// <param name="MaverickSkill"></param>
/// <param name="AnimationBuildupTime"></param>
/// <param name="DescriptionOverride"></param>
public sealed record BaseActionBuilder(
    int ID,
    string Name,
    Aether Cost,
    EActionType ActionType,
    ETargetType TargetType,
    ESkillType SkillType,
    List<EMonsterType> Types,
    List<EElement> Elements,
    EActionAnimationType AnimationType,
    Sprite ActionIconBig,
    Sprite ActionIconSmall,
    Sprite ActionIconCutSmall,
    bool FreeAction = false,
    bool MaverickSkill = false,
    float AnimationBuildupTime = 0f,
    string DescriptionOverride = ""
)
{
    public BaseAction Build()
    {
        BaseAction action = new()
        {
            ID = ID,
            Name = Name,
            Cost = Cost,
            ActionType = ActionType,
            TargetType = TargetType,
            SkillType = SkillType,
            ElementsOverride = Elements,
            Types =
            [
                .. Types.Select(x =>
                    MonsterTypes.TryGetObject(x, out GameObject typeGo) ? typeGo : null
                ),
            ],
            AnimationType = AnimationType,
            ActionIconBig = ActionIconBig,
            ActionIconSmall = ActionIconSmall,
            ActionIconCutSmall = ActionIconCutSmall,
            MaverickSkill = MaverickSkill,
            AnimationBuildupTime = AnimationBuildupTime,
            DescriptionOverride = DescriptionOverride,
        };

        if (FreeAction)
            action.SetFreeAction(true);

        return action;
    }
}
