using System.Collections.Generic;
using System.Threading.Tasks;
using Ethereal.Classes.LazyValues;
using UnityEngine;

namespace Ethereal.Classes.Builders;

/// <summary>
/// A helper record that creates a MetaUpgrade at runtime.
/// </summary>
/// <param name="ID"></param>
/// <param name="Name"></param>
/// <param name="Description"></param>
/// <param name="Sidenote"></param>
/// <param name="Cost"></param>
/// <param name="CostCurrency"></param>
/// <param name="UpgradeType"></param>
/// <param name="UnlockedMonster"></param>
/// <param name="RequiredMementos"></param>
/// <param name="RequiredSpecificMemento"></param>
/// <param name="RequiredArea"></param>
/// <param name="UnlockedExplorationAbility"></param>
/// <param name="PrerequirementUpgrades"></param>
public sealed record MetaUpgradeBuilder(
    int ID,
    string Name,
    string Description,
    string Sidenote,
    int Cost,
    ECollectibleLoot CostCurrency,
    EMetaUpgradeType UpgradeType,
    LazyMonster? UnlockedMonster = null,
    int RequiredMementos = 0,
    MonsterMemento? RequiredSpecificMemento = null,
    EArea? RequiredArea = null,
    EExplorationAbility? UnlockedExplorationAbility = null,
    List<GameObject>? PrerequirementUpgrades = null
)
{
    public async Task<MetaUpgrade> Build() =>
        new()
        {
            ID = ID,
            Name = Name,
            Description = Description,
            Sidenote = Sidenote,
            Cost = Cost,
            CostCurrency = CostCurrency,
            UpgradeType = UpgradeType,
            UnlockedMonster = UnlockedMonster is not null
                ? await UnlockedMonster.GetObject()
                : null,
            UnlockedExplorationAbility = UnlockedExplorationAbility ?? EExplorationAbility.None,
            RequiredMementos = RequiredMementos,
            RequiresMementosFromArea = RequiredArea.HasValue,
            RequiredSpecificMemento = RequiredSpecificMemento,
            RequiredArea = RequiredArea ?? EArea.PilgrimsRest,
            PrerequirementUpgrades = PrerequirementUpgrades ?? [],
        };
}
