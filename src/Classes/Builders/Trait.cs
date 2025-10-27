using System.Collections.Generic;
using System.Linq;
using Ethereal.API;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a Trait at runtime.
/// </summary>
/// <param name="ID"></param>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="Sidenote"></param>
/// <param name="SkillType"></param>
/// <param name="Types"></param>
/// <param name="PassiveEffects"></param>
/// <param name="Icon"></param>
/// <param name="Aura"></param>
/// <param name="MaverickSkill"></param>
public sealed record TraitBuilder(
    int ID,
    string Name,
    string Description,
    string Sidenote,
    ESkillType SkillType,
    List<EMonsterType> Types,
    List<PassiveEffect> PassiveEffects,
    Sprite Icon,
    bool Aura = false,
    bool MaverickSkill = false
)
{
    public Trait Build() =>
        new()
        {
            ID = ID,
            Name = Name,
            Description = Description,
            Sidenote = Sidenote,
            SkillType = SkillType,
            Types =
            [
                .. Types.Select(x =>
                    MonsterTypes.TryGetObject(x, out GameObject typeGo) ? typeGo : null
                ),
            ],
            PassiveEffectList = PassiveEffects,
            Icon = Icon,
            Aura = Aura,
            MaverickSkill = MaverickSkill,
        };
}
