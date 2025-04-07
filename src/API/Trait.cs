using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ethereal.API;

public static class Trait
{
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

    internal static void ReadQueue()
    {
        IsReady = true;

        while (Queue.TryDequeue(out var res))
            Add(res);

        while (QueueUpdate.TryDequeue(out var res))
            Update(res.id, res.descriptor);

        while (QueueUpdateByName.TryDequeue(out var res))
            Update(res.name, res.descriptor);
    }

    public static global::Trait Get(int id)
    {
        // Find trait by monster type
        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            global::Trait trait = type.Traits.Find(x => x.ID == id);

            if (trait != null)
                return trait;
        }

        // Find signature trait
        foreach (GameObject monster in GameController.Instance.CompleteMonsterList)
        {
            if (monster == null)
                continue;

            global::Trait trait = monster
                .GetComponent<SkillManager>()
                ?.SignatureTrait?.GetComponent<global::Trait>();

            if (trait.ID == id)
                return trait;
        }

        return null;
    }

    public static global::Trait Get(string name)
    {
        // Find trait by monster type
        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            global::Trait trait = type.Traits.Find(x => x.Name == name);

            if (trait != null)
                return trait;
        }

        // Find signature trait
        foreach (GameObject monster in GameController.Instance.CompleteMonsterList)
        {
            if (monster == null)
                continue;

            global::Trait trait = monster
                .GetComponent<SkillManager>()
                ?.SignatureTrait?.GetComponent<global::Trait>();

            if (trait.Name == name)
                return trait;
        }

        return null;
    }

    public static bool TryGet(int id, out global::Trait result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(id);

        return result != null;
    }

    public static bool TryGet(string name, out global::Trait result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(name);

        return result != null;
    }

    public static void Add(TraitDescriptor descriptor)
    {
        // Defer loading until ready
        if (GameController.Instance?.CompleteMonsterList == null || !IsReady)
        {
            Queue.Enqueue(descriptor);
            return;
        }

        var trait = new global::Trait()
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
                    GameController.Instance.MonsterTypes.Find(y => y?.Type == x)?.gameObject
                ),
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

            type.Traits.Add(go.GetComponent<global::Trait>());
        }

        WorldData.Instance.Referenceables.Add(go.GetComponent<global::Trait>());
    }

    public static void Update(int id, TraitDescriptor descriptor)
    {
        // Defer loading until ready
        if (GameController.Instance?.CompleteMonsterList == null || !IsReady)
        {
            QueueUpdate.Enqueue((id, descriptor));
            return;
        }

        if (TryGet(id, out var trait))
            Update(trait, descriptor);
    }

    public static void Update(string name, TraitDescriptor descriptor)
    {
        // Defer loading until ready
        if (GameController.Instance?.CompleteMonsterList == null || !IsReady)
        {
            QueueUpdateByName.Enqueue((name, descriptor));
            return;
        }

        if (TryGet(name, out var trait))
            Update(trait, descriptor);
    }

    private static void Update(global::Trait trait, TraitDescriptor descriptor)
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
