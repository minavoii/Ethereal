using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Ethereal.API;

public static class Monsters
{
    /// <summary>
    /// A helper class that describes a monster's properties.
    /// </summary>
    public class MonsterDescriptor
    {
        public string name = "";

        public EMonsterMainType? mainType;

        public List<EMonsterType> types = [];

        public (EElement first, EElement second) elements = (EElement.None, EElement.None);

        public List<(EElement element, int hits)> staggers = [];

        public string signatureTrait = "";

        public List<string> startingActions = [];

        public List<(string name, int multiplier)> perks = [];

        public int? baseMaxHealth;
    }

    private static bool IsReady = false;

    private static readonly ConcurrentQueue<(int id, MonsterDescriptor descriptor)> QueueUpdate =
        new();

    private static readonly ConcurrentQueue<(
        string name,
        MonsterDescriptor descriptor
    )> QueueUpdateByName = new();

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void ReadQueue()
    {
        IsReady = true;

        while (QueueUpdate.TryDequeue(out var item))
            Update(item.id, item.descriptor);

        while (QueueUpdateByName.TryDequeue(out var item))
            Update(item.name, item.descriptor);
    }

    /// <summary>
    /// Get a monster by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>an action if one was found; otherwise null.</returns>
    private static Monster Get(int id)
    {
        return GameController
            .Instance.CompleteMonsterList.Find(x => x?.GetComponent<Monster>()?.MonID == id)
            .GetComponent<Monster>();
    }

    /// <summary>
    /// Get a memento by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>an action if one was found; otherwise null.</returns>
    private static Monster Get(string name)
    {
        return GameController
            .Instance.CompleteMonsterList.Find(x => x?.GetComponent<Monster>()?.Name == name)
            .GetComponent<Monster>();
    }

    /// <summary>
    /// Get a memento by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and an artifact was found; otherwise, false.</returns>
    public static bool TryGet(int id, out Monster result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id);

        return result != null;
    }

    /// <summary>
    /// Get a memento by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and an artifact was found; otherwise, false.</returns>
    public static bool TryGet(string name, out Monster result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name);

        return result != null;
    }

    /// <summary>
    /// Overwrite a monster's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    public static void Update(int id, MonsterDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdate.Enqueue((id, descriptor));
            return;
        }

        if (TryGet(id, out var monster))
            Update(monster, descriptor);
    }

    /// <summary>
    /// Overwrite a monster's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    public static void Update(string name, MonsterDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdateByName.Enqueue((name, descriptor));
            return;
        }

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
        if (descriptor.name != string.Empty)
        {
            monster.Name = descriptor.name;
            monster.name = "Monster" + descriptor.name;
        }

        if (
            descriptor.elements.first != EElement.None
            && descriptor.elements.second != EElement.None
        )
        {
            monster.GetComponent<SkillManager>().Elements[0] = descriptor.elements.first;
            monster.GetComponent<SkillManager>().Elements[1] = descriptor.elements.second;
        }

        if (descriptor.mainType.HasValue)
            monster.GetComponent<SkillManager>().MainType = descriptor.mainType.Value;

        if (descriptor.types.Count == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                MonsterType type = GameController.Instance.MonsterTypes.Find(x =>
                    x?.Type == descriptor.types[i]
                );

                GameObject go = Utils.Converter.IntoGameObject(type);
                monster.GetComponent<SkillManager>().MonsterTypes[i] = go;
            }
        }

        if (descriptor.staggers.Count != 0)
        {
            monster.GetComponent<SkillManager>().StaggerDefines.Clear();

            foreach (var (element, hits) in descriptor.staggers)
            {
                monster
                    .GetComponent<SkillManager>()
                    .StaggerDefines.Add(new() { Element = element, Hits = hits });
            }
        }

        if (descriptor.signatureTrait != string.Empty)
        {
            if (Traits.TryGet(descriptor.signatureTrait, out var trait))
                monster.GetComponent<SkillManager>().SignatureTrait = trait.gameObject;
        }

        if (descriptor.startingActions.Count != 0)
        {
            monster.GetComponent<SkillManager>().StartActions.Clear();

            foreach (string actionName in descriptor.startingActions)
            {
                if (Actions.TryGet(actionName, out var action))
                    monster.GetComponent<SkillManager>().StartActions.Add(action.gameObject);
            }
        }

        if (descriptor.perks.Count == 3)
        {
            monster.GetComponent<MonsterStats>().PerkInfosList.Clear();

            foreach ((string perkName, int multiplier) in descriptor.perks)
            {
                PerkInfos perk = GetPerk(perkName);
                perk.Multiplier = multiplier;

                if (perk != null)
                    monster.GetComponent<MonsterStats>().PerkInfosList.Add(perk);
            }
        }

        if (descriptor.baseMaxHealth.HasValue)
            monster.GetComponent<MonsterStats>().BaseMaxHealth = descriptor.baseMaxHealth.Value;
    }

    private static PerkInfos GetPerk(string name)
    {
        // Find perk by monster
        foreach (GameObject monster in GameController.Instance.CompleteMonsterList)
        {
            if (monster == null)
                continue;

            foreach (PerkInfos perk in monster.GetComponent<MonsterStats>().PerkInfosList)
            {
                if (perk.Perk.GetComponent<Perk>().Name == name)
                    return perk;
            }
        }

        return null;
    }
}
