using System;
using System.Collections.Generic;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;
using Ethereal.Classes.LazyValues;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Monsters
{
    /// <summary>
    /// A helper class that describes a monster's properties.
    /// </summary>
    public class MonsterDescriptor
    {
        public string? Name { get; init; }

        public string? Description { get; init; }

        public EMonsterMainType? MainType { get; init; }

        public List<EMonsterType>? Types { get; init; }

        public (EElement first, EElement second)? Elements { get; init; }

        public List<(EElement element, int hits)>? Staggers { get; init; }

        public List<(EElement element, int hits)>? BossStaggers { get; init; }

        public LazyTrait? SignatureTrait { get; init; }

        public List<LazyAction>? StartingActions { get; init; }

        public List<PerkInfosBuilder>? Perks { get; init; }

        public int? BaseMaxHealth { get; init; }

        public List<MonsterAIActionBuilder>? Scripting { get; init; }

        public List<MonsterAITraitBuilder>? WildTraits { get; init; }

        public LazyTrait? EliteTrait { get; init; }

        public MonsterAnimator? Animator { get; init; }

        public MonsterShiftBuilder? Shift { get; init; }
    }

    /// <summary>
    /// Get a monster by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static Monster? Get(int id) =>
        Get(x =>
            x?.GetComponent<Monster>() is Monster monster
            && (monster.ID == id || monster.MonID == id)
        );

    /// <summary>
    /// Get a monster by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Monster? Get(string name) => Get(x => x?.GetComponent<Monster>()?.Name == name);

    private static Monster? Get(Predicate<GameObject?> predicate) =>
        GameController.Instance.CompleteMonsterList.Find(predicate)?.GetComponent<Monster>();

    /// <summary>
    /// Create a new monster and add it to the game's data.
    /// </summary>
    [Deferrable]
    private static void Add_Impl(MonsterBuilder monsterBuilder) => Add_Impl(monsterBuilder.Build());

    /// <summary>
    /// Create a new monster and add it to the game's data.
    /// </summary>
    [Deferrable]
    private static void Add_Impl(GameObject monster)
    {
        GameController.Instance.CompleteMonsterList.Add(monster);
        GameController.Instance.ActiveMonsterList.Add(monster);
        Referenceables.Add(monster.GetComponent<Monster>());
    }

    /// <summary>
    /// Overwrite a monster's properties with values from a descriptor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="descriptor"></param>
    [Deferrable]
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
    [Deferrable]
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
        if (descriptor.Name is not null)
        {
            monster.Name = descriptor.Name;
            monster.name = "Monster" + descriptor.Name;
        }

        if (descriptor.Description is not null)
            monster.Description = descriptor.Description;

        if (descriptor.Elements is (EElement, EElement) elements && elements.first != EElement.None)
        {
            SkillManager skillManager = monster.GetComponent<SkillManager>();

            skillManager.Elements[0] = elements.first;

            // Remove the second element
            if (elements.second == EElement.None)
            {
                if (skillManager.Elements.Count > 1)
                    skillManager.Elements.RemoveAt(1);
            }
            // Replace the second element
            else if (skillManager.Elements.Count > 1)
                skillManager.Elements[1] = elements.second;
            // Add a second element because none was found
            else
                skillManager.Elements.Add(elements.second);
        }

        if (descriptor.MainType.HasValue)
            monster.GetComponent<SkillManager>().MainType = descriptor.MainType.Value;

        if (descriptor.Types?.Count == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject go = MonsterTypes.NativeTypes[descriptor.Types[i]];
                monster.GetComponent<SkillManager>().MonsterTypes[i] = go;
            }
        }

        if (descriptor.Staggers?.Count > 0)
        {
            monster.GetComponent<SkillManager>().StaggerDefines.Clear();

            foreach (var (element, hits) in descriptor.Staggers)
            {
                monster
                    .GetComponent<SkillManager>()
                    .StaggerDefines.Add(new() { Element = element, Hits = hits });
            }
        }

        if (descriptor.BossStaggers?.Count > 0)
        {
            monster.GetComponent<SkillManager>().BossStagger.Clear();

            foreach (var (element, hits) in descriptor.BossStaggers)
            {
                monster
                    .GetComponent<SkillManager>()
                    .BossStagger.Add(new() { Element = element, Hits = hits });
            }
        }

        if (descriptor.SignatureTrait is not null)
            monster.GetComponent<SkillManager>().SignatureTrait = descriptor
                .SignatureTrait.Get()
                ?.gameObject;

        if (descriptor.StartingActions?.Count > 0)
        {
            monster.GetComponent<SkillManager>().StartActions.Clear();

            foreach (LazyAction lazyAction in descriptor.StartingActions)
                if (lazyAction.Get() is BaseAction action)
                    monster.GetComponent<SkillManager>().StartActions.Add(action.gameObject);
        }

        if (descriptor.Perks?.Count == 3)
        {
            monster.GetComponent<MonsterStats>().PerkInfosList.Clear();

            foreach (PerkInfosBuilder perkInfosBuilder in descriptor.Perks)
                if (perkInfosBuilder.Build() is PerkInfos perkInfos)
                    monster.GetComponent<MonsterStats>().PerkInfosList.Add(perkInfos);
        }

        if (descriptor.BaseMaxHealth.HasValue)
            monster.GetComponent<MonsterStats>().BaseMaxHealth = descriptor.BaseMaxHealth.Value;

        if (descriptor.Scripting?.Count > 0)
        {
            MonsterAI ai = monster.GetComponent<MonsterAI>();
            ai.Scripting.Clear();

            foreach (MonsterAIActionBuilder actionBuilder in descriptor.Scripting)
                if (actionBuilder.Build() is MonsterAIAction action)
                    ai.Scripting.Add(action);
        }

        if (descriptor.WildTraits?.Count > 0)
        {
            MonsterAI ai = monster.GetComponent<MonsterAI>();
            ai.Traits.Clear();

            foreach (MonsterAITraitBuilder traitBuilder in descriptor.WildTraits)
                if (traitBuilder.Build() is MonsterAI.MonsterAITrait trait)
                    ai.Traits.Add(trait);
        }

        if (descriptor.EliteTrait?.Get() is Trait eliteTrait)
            monster.GetComponent<SkillManager>().EliteTrait = eliteTrait.gameObject;

        if (descriptor.Animator is MonsterAnimator newAnim)
        {
            MonsterAnimator anim = monster.GetComponent<MonsterAnimator>();

            anim.CombatAttack = newAnim.Ambush ?? anim.Ambush;
            anim.CombatAttack = newAnim.CombatAttack ?? anim.CombatAttack;
            anim.CombatAttackEnd = newAnim.CombatAttackEnd ?? anim.CombatAttackEnd;
            anim.CombatAttackJump = newAnim.CombatAttackJump ?? anim.CombatAttackJump;
            anim.CombatCast = newAnim.CombatCast ?? anim.CombatCast;
            anim.CombatCastEnd = newAnim.CombatCastEnd ?? anim.CombatCastEnd;
            anim.CombatCastStart = newAnim.CombatCastStart ?? anim.CombatCastStart;
            anim.CombatDeath = newAnim.CombatDeath ?? anim.CombatDeath;
            anim.CombatHit = newAnim.CombatHit ?? anim.CombatHit;
            anim.CombatIdle = newAnim.CombatIdle ?? anim.CombatIdle;
            anim.CombatIntro = newAnim.CombatIntro ?? anim.CombatIntro;
            anim.CombatJumpBack = newAnim.CombatJumpBack ?? anim.CombatJumpBack;
            anim.CombatOrderEnd = newAnim.CombatOrderEnd ?? anim.CombatOrderEnd;
            anim.CombatOrderLoop = newAnim.CombatOrderLoop ?? anim.CombatOrderLoop;
            anim.CombatOrderStart = newAnim.CombatOrderStart ?? anim.CombatOrderStart;
            anim.CombatSelectCancel = newAnim.CombatSelectCancel ?? anim.CombatSelectCancel;
            anim.CombatSelectLoop = newAnim.CombatSelectLoop ?? anim.CombatSelectLoop;
            anim.CombatSelectTransition =
                newAnim.CombatSelectTransition ?? anim.CombatSelectTransition;
            anim.FallDown = newAnim.FallDown ?? anim.FallDown;
            anim.FallDownDiagonal = newAnim.FallDownDiagonal ?? anim.FallDownDiagonal;
            anim.FallHorizontal = newAnim.FallHorizontal ?? anim.FallHorizontal;
            anim.FallTop = newAnim.FallTop ?? anim.FallTop;
            anim.FallTopDiagonal = newAnim.FallTopDiagonal ?? anim.FallTopDiagonal;
            anim.Hidden = newAnim.Hidden ?? anim.Hidden;
            anim.IdleDown = newAnim.IdleDown ?? anim.IdleDown;
            anim.IdleDownDiagonal = newAnim.IdleDownDiagonal ?? anim.IdleDownDiagonal;
            anim.IdleHorizontal = newAnim.IdleHorizontal ?? anim.IdleHorizontal;
            anim.IdleTop = newAnim.IdleTop ?? anim.IdleTop;
            anim.IdleTopDiagonal = newAnim.IdleTopDiagonal ?? anim.IdleTopDiagonal;
            anim.RunDown = newAnim.RunDown ?? anim.RunDown;
            anim.RunDownDiagonal = newAnim.RunDownDiagonal ?? anim.RunDownDiagonal;
            anim.RunHorizontal = newAnim.RunHorizontal ?? anim.RunHorizontal;
            anim.RunTop = newAnim.RunTop ?? anim.RunTop;
            anim.RunTopDiagonal = newAnim.RunTopDiagonal ?? anim.RunTopDiagonal;
            anim.RunTransitionDown = newAnim.RunTransitionDown ?? anim.RunTransitionDown;
            anim.RunTransitionDownDiagonal =
                newAnim.RunTransitionDownDiagonal ?? anim.RunTransitionDownDiagonal;
            anim.RunTransitionHorizontal =
                newAnim.RunTransitionHorizontal ?? anim.RunTransitionHorizontal;
            anim.RunTransitionTop = newAnim.RunTransitionTop ?? anim.RunTransitionTop;
            anim.RunTransitionTopDiagonal =
                newAnim.RunTransitionTopDiagonal ?? anim.RunTransitionTopDiagonal;

            anim.UseCastingAnimations = newAnim.UseCastingAnimations || anim.UseCastingAnimations;
            anim.DisableHitAnimation = newAnim.DisableHitAnimation || anim.DisableHitAnimation;
            anim.ExplorationSpritesheet =
                newAnim.ExplorationSpritesheet ?? anim.ExplorationSpritesheet;
            anim.Texture = newAnim.Texture ?? anim.Texture;
        }

        if (descriptor.Shift?.Build() is MonsterShift shift)
        {
            var monsterShift = monster.GetComponent<MonsterShift>();

            monsterShift.MonsterTypesOverride = shift.MonsterTypesOverride;
            monsterShift.ElementsOverride = shift.ElementsOverride;
            monsterShift.SignatureTraitOverride = shift.SignatureTraitOverride;
            monsterShift.StartActionsOverride = shift.StartActionsOverride;
            monsterShift.ResetPoiseActionOverride = shift.ResetPoiseActionOverride;
            monsterShift.PerksOverride = shift.PerksOverride;

            monsterShift.ChangeMainType = shift.ChangeMainType;
            monsterShift.MainTypeOverride = shift.MainTypeOverride;

            monsterShift.ChangeHealth = shift.ChangeHealth;
            monsterShift.HealthOverride = shift.HealthOverride;

            monsterShift.BattleSpriteSource = shift.BattleSpriteSource;
            monsterShift.BattleSpriteReplace = shift.BattleSpriteReplace;
            monsterShift.OverworldSpriteSource = shift.OverworldSpriteSource;
            monsterShift.OverworldSpriteReplace = shift.OverworldSpriteReplace;
            monsterShift.ProjectileSpriteSource = shift.ProjectileSpriteSource;
            monsterShift.ProjectileSpriteReplace = shift.ProjectileSpriteReplace;

            monsterShift.PortraitSprite = shift.PortraitSprite;
            monsterShift.ShiftVFX = shift.ShiftVFX;
        }
    }
}
