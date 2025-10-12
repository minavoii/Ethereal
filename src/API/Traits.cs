using System.Collections.Generic;
using System.Linq;
using Ethereal.Generator;
using UnityEngine;

namespace Ethereal.API;

[Deferreable]
public static partial class Traits
{
    /// <summary>
    /// A helper class that describes a trait's properties.
    /// </summary>
    public class TraitDescriptor()
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public string Sidenote { get; set; } = "";

        public bool? Aura { get; set; }

        public bool? MaverickSkill { get; set; }

        public List<PassiveEffect> PassiveEffects { get; set; } = [];

        public List<EMonsterType> Types { get; set; } = [];

        public Sprite Icon { get; set; }

        public ESkillType? SkillType { get; set; }
    }

    /// <summary>
    /// Get a trait by id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>a trait if one was found; otherwise null.</returns>
    [TryGet]
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
        foreach (GameObject monster in GameController.Instance.ActiveMonsterList)
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
    [TryGet]
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
        foreach (GameObject monster in GameController.Instance.ActiveMonsterList)
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
    /// Get all traits that can be learned (i.e. non-signature traits).
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<Trait> GetAllLearnable()
    {
        List<Trait> traits = [];

        foreach (MonsterType type in GameController.Instance.MonsterTypes)
        {
            foreach (Trait trait in type.Traits)
            {
                if (trait != null && !traits.Contains(trait))
                    traits.Add(trait);
            }
        }

        return traits;
    }

    /// <summary>
    /// Get all signature traits.
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<Trait> GetAllSignature()
    {
        return
        [
            .. GameController
                .Instance.ActiveMonsterList.Select(x =>
                    x.GetComponent<SkillManager>()?.SignatureTrait?.GetComponent<Trait>()
                )
                .Where(x => x.Name != "?????"),
        ];
    }

    /// <summary>
    /// Get all traits, both learnable and signature ones.
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<Trait> GetAll()
    {
        List<Trait> traits = [.. GetAllLearnable().Concat(GetAllSignature())];

        return traits;
    }

    /// <summary>
    /// Create a new trait and add it to the game's data.
    /// </summary>
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Add_Impl(TraitDescriptor descriptor)
    {
        var trait = new Trait()
        {
            ID = descriptor.Id,
            Name = descriptor.Name,
            Description = descriptor.Description,
            Sidenote = descriptor.Sidenote,
            Aura = descriptor.Aura ?? false,
            MaverickSkill = descriptor.MaverickSkill ?? false,
            PassiveEffectList = descriptor.PassiveEffects,
            Types =
            [
                .. descriptor.Types.Select(x =>
                {
                    MonsterTypes.TryGet(x, out var type);
                    return type?.gameObject;
                }),
            ],
            Icon = descriptor.Icon,
            SkillType = descriptor.SkillType ?? ESkillType.Shared,
        };

        var go = Utils.GameObjects.IntoGameObject(trait);

        foreach (EMonsterType monsterType in descriptor.Types)
        {
            if (MonsterTypes.TryGet(monsterType, out var type))
                type.Traits.Add(go.GetComponent<Trait>());
        }

        WorldData.Instance.Referenceables.Add(go.GetComponent<Trait>());
    }

    /// <summary>
    /// Overwrite a trait's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Update_Impl(int id, TraitDescriptor descriptor)
    {
        if (TryGet(id, out var trait))
            Update(trait, descriptor);
    }

    /// <summary>
    /// Overwrite a trait's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    [Deferreable]
    private static void Update_Impl(string name, TraitDescriptor descriptor)
    {
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
        if (descriptor.Name != string.Empty)
            trait.Name = descriptor.Name;

        if (descriptor.Description != string.Empty)
            trait.Description = descriptor.Description;

        if (descriptor.Sidenote != string.Empty)
            trait.Sidenote = descriptor.Sidenote;

        if (descriptor.Aura.HasValue)
            trait.Aura = descriptor.Aura.Value;

        if (descriptor.MaverickSkill.HasValue)
            trait.MaverickSkill = descriptor.MaverickSkill.Value;

        if (descriptor.PassiveEffects.Count != 0)
        {
            foreach (PassiveEffect comp in trait.GetComponents<PassiveEffect>())
                Object.DestroyImmediate(comp);

            GameObject go = trait.gameObject;

            foreach (PassiveEffect passive in descriptor.PassiveEffects)
                Utils.GameObjects.CopyToGameObject(ref go, passive);

            trait.InitializeReferenceable();
        }

        if (descriptor.Types.Count != 0)
        {
            trait.Types.Clear();

            foreach (var monsterType in descriptor.Types)
            {
                if (MonsterTypes.TryGet(monsterType, out var type))
                    trait.Types.Add(Utils.GameObjects.IntoGameObject(type));
            }
        }

        if (descriptor.Icon != null)
            trait.Icon = descriptor.Icon;

        if (descriptor.SkillType.HasValue)
            trait.SkillType = descriptor.SkillType.Value;
    }
}
