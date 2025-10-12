using System.Collections.Generic;
using Ethereal.Generator;
using UnityEngine;

namespace Ethereal.API;

[Deferreable]
public static partial class Monsters
{
    /// <summary>
    /// A helper class that describes a monster's properties.
    /// </summary>
    public class MonsterDescriptor
    {
        public string Name { get; set; } = "";

        public EMonsterMainType? MainType { get; set; }

        public List<EMonsterType> Types { get; set; } = [];

        public (EElement first, EElement second) Elements { get; set; } =
            (EElement.None, EElement.None);

        public List<(EElement element, int hits)> Staggers { get; set; } = [];

        public Trait SignatureTrait { get; set; }

        public List<BaseAction> StartingActions { get; set; } = [];

        public List<PerkInfos> Perks { get; set; } = [];

        public int? BaseMaxHealth { get; set; }

        public List<MonsterAIAction> Scripting { get; set; } = [];

        public List<MonsterAI.MonsterAITrait> WildTraits { get; set; } = [];

        public Trait EliteTrait { get; set; }
    }

    /// <summary>
    /// Get a monster by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>an action if one was found; otherwise null.</returns>
    [TryGet]
    private static Monster Get(int id)
    {
        return GameController
            .Instance.ActiveMonsterList.Find(x => x?.GetComponent<Monster>()?.MonID == id)
            ?.GetComponent<Monster>();
    }

    /// <summary>
    /// Get a memento by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>an action if one was found; otherwise null.</returns>
    [TryGet]
    private static Monster Get(string name)
    {
        return GameController
            .Instance.ActiveMonsterList.Find(x => x?.GetComponent<Monster>()?.Name == name)
            ?.GetComponent<Monster>();
    }

    /// <summary>
    /// Overwrite a monster's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Update_Impl(int id, MonsterDescriptor descriptor)
    {
        if (TryGet(id, out var monster))
            Update(monster, descriptor);
    }

    /// <summary>
    /// Overwrite a monster's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Update_Impl(string name, MonsterDescriptor descriptor)
    {
        if (TryGet(name, out var monster))
            Update(monster, descriptor);
    }

    /// <summary>
    /// Overwrite a monster's properties with values from a descriptor.
    /// </summary>
    /// <param name="monster"></param>
    /// <param name="descriptor"></param>
    private static void Update(Monster monster, MonsterDescriptor descriptor)
    {
        if (descriptor.Name != string.Empty)
        {
            monster.Name = descriptor.Name;
            monster.name = "Monster" + descriptor.Name;
        }

        if (
            descriptor.Elements.first != EElement.None
            && descriptor.Elements.second != EElement.None
        )
        {
            monster.GetComponent<SkillManager>().Elements[0] = descriptor.Elements.first;
            monster.GetComponent<SkillManager>().Elements[1] = descriptor.Elements.second;
        }

        if (descriptor.MainType.HasValue)
            monster.GetComponent<SkillManager>().MainType = descriptor.MainType.Value;

        if (descriptor.Types.Count == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject go = MonsterTypes.NativeTypes[descriptor.Types[i]];
                monster.GetComponent<SkillManager>().MonsterTypes[i] = go;
            }
        }

        if (descriptor.Staggers.Count != 0)
        {
            monster.GetComponent<SkillManager>().StaggerDefines.Clear();

            foreach (var (element, hits) in descriptor.Staggers)
            {
                monster
                    .GetComponent<SkillManager>()
                    .StaggerDefines.Add(new() { Element = element, Hits = hits });
            }
        }

        if (descriptor.SignatureTrait != null)
            monster.GetComponent<SkillManager>().SignatureTrait = descriptor
                .SignatureTrait
                .gameObject;

        if (descriptor.StartingActions.Count != 0)
        {
            monster.GetComponent<SkillManager>().StartActions.Clear();

            foreach (BaseAction action in descriptor.StartingActions)
                monster.GetComponent<SkillManager>().StartActions.Add(action.gameObject);
        }

        if (descriptor.Perks.Count == 3)
        {
            monster.GetComponent<MonsterStats>().PerkInfosList.Clear();

            foreach (PerkInfos perk in descriptor.Perks)
                monster.GetComponent<MonsterStats>().PerkInfosList.Add(perk);
        }

        if (descriptor.BaseMaxHealth.HasValue)
            monster.GetComponent<MonsterStats>().BaseMaxHealth = descriptor.BaseMaxHealth.Value;

        if (descriptor.Scripting.Count != 0)
        {
            MonsterAI ai = monster.GetComponent<MonsterAI>();
            ai.Scripting.Clear();

            foreach (MonsterAIAction action in descriptor.Scripting)
                ai.Scripting.Add(action);
        }

        if (descriptor.WildTraits.Count != 0)
        {
            MonsterAI ai = monster.GetComponent<MonsterAI>();
            ai.Traits.Clear();

            foreach (MonsterAI.MonsterAITrait trait in descriptor.WildTraits)
                ai.Traits.Add(trait);
        }

        if (descriptor.EliteTrait != null)
            monster.GetComponent<SkillManager>().EliteTrait = descriptor.EliteTrait.gameObject;
    }
}
