using System.Collections.Generic;
using System.Linq;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a SkillManager at runtime.
/// </summary>
/// <param name="MainType"></param>
/// <param name="SignatureTrait"></param>
/// <param name="MonsterTypes"></param>
/// <param name="Elements"></param>
/// <param name="StaggerDefines"></param>
/// <param name="StartActions"></param>
/// <param name="EliteTrait"></param>
/// <param name="BossStagger"></param>
/// <param name="BossAlternativeStagger"></param>
/// <param name="ImpossibleToStagger"></param>
/// <param name="AllAetherDefaultAttack"></param>
public sealed record SkillManagerBuilder(
    EMonsterMainType MainType,
    LazyTrait SignatureTrait,
    List<EMonsterType> MonsterTypes,
    List<EElement> Elements,
    List<StaggerDefine> StaggerDefines,
    List<LazyAction> StartActions,
    LazyTrait EliteTrait,
    List<StaggerDefine> BossStagger,
    List<StaggerDefine> BossAlternativeStagger,
    bool ImpossibleToStagger,
    bool AllAetherDefaultAttack
)
{
    public SkillManager Build() =>
        new()
        {
            MainType = MainType,
            SignatureTrait = SignatureTrait.Get()?.gameObject,
            MonsterTypes =
            [
                .. MonsterTypes.Select(x =>
                    Ethereal.API.MonsterTypes.TryGetObject(x, out var res) ? res : null
                ),
            ],
            Elements = Elements,
            StaggerDefines = StaggerDefines,
            StartActions = [.. StartActions.Select(x => x.Get()?.gameObject)],
            EliteTrait = EliteTrait.Get()?.gameObject,
            BossStagger = BossStagger,
            BossAlternativeStagger = BossAlternativeStagger,
            ImpossibleToStagger = ImpossibleToStagger,
            AllAetherDefaultAttack = AllAetherDefaultAttack,
        };
}
