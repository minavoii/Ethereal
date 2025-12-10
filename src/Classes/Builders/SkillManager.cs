using System.Collections.Generic;
using System.Linq;
using Ethereal.API;
using Ethereal.Classes.LazyValues;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a SkillManager at runtime.
/// </summary>
/// <param name="MainType"></param>
/// <param name="SignatureTrait"></param>
/// <param name="Types"></param>
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
    List<EMonsterType> Types,
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
                .. Types.Select(x =>
                    MonsterTypes.TryGet(x, out MonsterType type) ? type.gameObject : null
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
