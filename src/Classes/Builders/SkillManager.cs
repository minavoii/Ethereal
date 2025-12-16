using System.Collections.Generic;
using System.Threading.Tasks;
using Ethereal.API;
using Ethereal.Classes.LazyValues;
using Ethereal.Utils.Extensions;

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
    public async Task<SkillManager> Build() =>
        new()
        {
            MainType = MainType,
            SignatureTrait = await SignatureTrait.GetObject(),
            MonsterTypes =
            [
                .. await Types.SelectAsync(async x => (await MonsterTypes.Get(x))?.gameObject),
            ],
            Elements = Elements,
            StaggerDefines = StaggerDefines,
            StartActions =
            [
                .. await StartActions.SelectAsync(async x => (await x.Get())?.gameObject),
            ],
            EliteTrait = (await EliteTrait.Get())?.gameObject,
            BossStagger = BossStagger,
            BossAlternativeStagger = BossAlternativeStagger,
            ImpossibleToStagger = ImpossibleToStagger,
            AllAetherDefaultAttack = AllAetherDefaultAttack,
        };
}
