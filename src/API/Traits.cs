using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ethereal.API;

public static class Traits
{
    /// <summary>
    /// A helper class that describes a trait's properties.
    /// </summary>
    public class TraitDescriptor()
    {
        public int id;

        public string name = "";

        public string description = "";

        public string sidenote = "";

        public bool? aura;

        public bool? maverickSkill;

        public List<PassiveEffect> passiveEffects = [];

        public List<EMonsterType> types = [];

        public Sprite icon;

        public ESkillType? skillType;
    }

    private static readonly ConcurrentQueue<TraitDescriptor> Queue = new();

    private static readonly ConcurrentQueue<(int id, TraitDescriptor descriptor)> QueueUpdate =
        new();

    private static readonly ConcurrentQueue<(
        string name,
        TraitDescriptor descriptor
    )> QueueUpdateByName = new();

    private static bool IsReady = false;

    /// <summary>
    /// Mark the API as ready and run all deferred methods.
    /// </summary>
    internal static void SetReady()
    {
        IsReady = true;

        while (Queue.TryDequeue(out var item))
            Add(item);

        while (QueueUpdate.TryDequeue(out var item))
            Update(item.id, item.descriptor);

        while (QueueUpdateByName.TryDequeue(out var item))
            Update(item.name, item.descriptor);
    }

    /// <summary>
    /// Get a trait by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>a trait if one was found; otherwise null.</returns>
    private static Trait Get(int id)
    {
        // Find trait by monster type
        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            Trait trait = type.Traits.Find(x => x.ID == id);

            if (trait != null)
                return trait;
        }

        // Find signature trait
        foreach (GameObject monster in GameController.Instance.CompleteMonsterList)
        {
            if (monster == null)
                continue;

            Trait trait = monster
                .GetComponent<SkillManager>()
                ?.SignatureTrait?.GetComponent<Trait>();

            if (trait.ID == id)
                return trait;
        }

        return null;
    }

    /// <summary>
    /// Get a trait by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>a trait if one was found; otherwise null.</returns>
    private static Trait Get(string name)
    {
        // Find trait by monster type
        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            Trait trait = type.Traits.Find(x => x.Name == name);

            if (trait != null)
                return trait;
        }

        // Find signature trait
        foreach (GameObject monster in GameController.Instance.CompleteMonsterList)
        {
            if (monster == null)
                continue;

            Trait trait = monster
                .GetComponent<SkillManager>()
                ?.SignatureTrait?.GetComponent<Trait>();

            if (trait.Name == name)
                return trait;
        }

        return null;
    }

    /// <summary>
    /// Get a trait by id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and a trait was found; otherwise, false.</returns>
    public static bool TryGet(int id, out Trait result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id);

        return result != null;
    }

    /// <summary>
    /// Get a trait by name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns>true if the API is ready and a trait was found; otherwise, false.</returns>
    public static bool TryGet(string name, out Trait result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name);

        return result != null;
    }

    /// <summary>
    /// Create a new trait and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    public static void Add(TraitDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            Queue.Enqueue(descriptor);
            return;
        }

        var trait = new Trait()
        {
            ID = descriptor.id,
            Name = descriptor.name,
            Description = descriptor.description,
            Sidenote = descriptor.sidenote,
            Aura = descriptor.aura ?? false,
            MaverickSkill = descriptor.maverickSkill ?? false,
            PassiveEffectList = descriptor.passiveEffects,
            Types =
            [
                .. descriptor.types.Select(x =>
                {
                    MonsterTypes.TryGet(x, out var type);
                    return type?.gameObject;
                }),
            ],
            Icon = descriptor.icon,
            SkillType = descriptor.skillType ?? ESkillType.Shared,
        };

        var go = Utils.Converter.IntoGameObject(trait);

        foreach (EMonsterType monsterType in descriptor.types)
        {
            MonsterType type = GameController.Instance.MonsterTypes.Find(x =>
                x?.Type == monsterType
            );

            type.Traits.Add(go.GetComponent<Trait>());
        }

        WorldData.Instance.Referenceables.Add(go.GetComponent<Trait>());
    }

    /// <summary>
    /// Overwrite a trait's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    public static void Update(int id, TraitDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdate.Enqueue((id, descriptor));
            return;
        }

        if (TryGet(id, out var trait))
            Update(trait, descriptor);
    }

    /// <summary>
    /// Overwrite a trait's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    public static void Update(string name, TraitDescriptor descriptor)
    {
        // Defer loading until ready
        if (!IsReady)
        {
            QueueUpdateByName.Enqueue((name, descriptor));
            return;
        }

        if (TryGet(name, out var trait))
            Update(trait, descriptor);
    }

    /// <summary>
    /// Overwrite a trait's properties with values from a descriptor.
    /// </summary>
    /// <param name="trait"></param>
    /// <param name="descriptor"></param>
    private static void Update(Trait trait, TraitDescriptor descriptor)
    {
        if (descriptor.name != string.Empty)
            trait.Name = descriptor.name;

        if (descriptor.description != string.Empty)
            trait.Description = descriptor.description;

        if (descriptor.sidenote != string.Empty)
            trait.Sidenote = descriptor.sidenote;

        if (descriptor.aura.HasValue)
            trait.Aura = descriptor.aura.Value;

        if (descriptor.maverickSkill.HasValue)
            trait.MaverickSkill = descriptor.maverickSkill.Value;

        if (descriptor.passiveEffects.Count != 0)
        {
            foreach (PassiveEffect comp in trait.GetComponents<PassiveEffect>())
            {
                Object.DestroyImmediate(comp);
            }

            GameObject go = trait.gameObject;

            foreach (PassiveEffect passive in descriptor.passiveEffects)
            {
                Utils.Converter.CopyToGameObject(ref go, passive);
            }

            trait.InitializeReferenceable();
        }

        if (descriptor.types.Count != 0)
        {
            trait.Types.Clear();

            foreach (var monsterType in descriptor.types)
            {
                MonsterType type = GameController.Instance.MonsterTypes.Find(x =>
                    x?.Type == monsterType
                );
                GameObject go = Utils.Converter.IntoGameObject(type);

                trait.Types.Add(go);
            }
        }

        if (descriptor.icon != null)
            trait.Icon = descriptor.icon;

        if (descriptor.skillType.HasValue)
            trait.SkillType = descriptor.skillType.Value;
    }
}
