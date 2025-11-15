using System.IO;
using Ethereal.API;
using Ethereal.Classes.Builders;
using ExampleMonsters.CustomActions;
using ExampleMonsters.CustomTraits;
using UnityEngine;

namespace ExampleMonsters.CustomMonsters;

internal static class WaterWheel
{
    private const int ID = 8001;

    private const int MonID = 4002;

    private const int MementoID = 1907;

    private const int MementoShiftedID = 1908;

    private const int MetaUpgradeID = 9100;

    private const string Name = "Water Wheel";

    private static readonly AnimationClip CombatIdle = Animations.LoadFromAsset(
        Path.Join(Plugin.CustomMonstersPath, "WaterWheel_CombatIdle"),
        "assets/animations/waterwheel/waterwheel_b_0.prefab"
    );

    private static readonly AnimationClip CombatCast = Animations.LoadFromAsset(
        Path.Join(Plugin.CustomMonstersPath, "WaterWheel_Cast"),
        "assets/animations/waterwheel/waterwheel_cast.prefab"
    );

    private static readonly AnimationClip CombatCastEnd = Animations.LoadFromAsset(
        Path.Join(Plugin.CustomMonstersPath, "WaterWheel_CastEnd"),
        "assets/animations/waterwheel/waterwheel_castend2.prefab"
    );

    private static readonly Texture2D Texture = new(0, 0);

    private static readonly Texture2D ExplorationSpritesheet = new(0, 0);

    internal static MonsterBuilder Builder =>
        new(Monster, Animator, Bounds, SkillManager, Stats, AI, OverworldBehaviour, Shift);

    private static readonly Monster Monster = new()
    {
        ID = ID,
        MonID = MonID,
        Name = Name,
        Description = "",
        PortraitSpriteSmall = new(),
        MonsterTriggerIcon = new(),
        UseCustomCombatPosition = false,
        AttackAnimationType = EAttackAnimationType.Jump,
        UseCustomAttackCurve = false,
        longAttack = false,
        EnemyMonsterHUDOverwrite = null,
        DeleteGameobjectOnDeath = false,
    };

    private static readonly MonsterBounds Bounds = new(new(0f, 125f), null, null);

    private static readonly MonsterAnimator Animator = new()
    {
        CombatIdle = CombatIdle,
        CombatAttackJump = CombatIdle,
        CombatAttack = CombatCast,
        CombatAttackEnd = CombatIdle,
        CombatJumpBack = CombatIdle,
        CombatCastStart = CombatCastEnd,
        CombatCast = CombatCast,
        CombatCastEnd = CombatCastEnd,
        CombatHit = GameObject.Instantiate(CombatIdle),
        CombatDeath = CombatCastEnd,
        Texture = Texture,
        ExplorationSpritesheet = ExplorationSpritesheet,
        DisableHitAnimation = true,
    };

    private static readonly OverworldMonsterBehaviour OverworldBehaviour = new()
    {
        OverWorldBehaviour = EOverWorldBehaviour.Normal,
        IdleTime = new(1f, 2f),
        MovementSpeed = new(2.5f, 3.5f),
        MovementDistance = new(2.5f, 5f),
        NoIdleChance = 0.25f,
        AmbushMonsterTriggerMultiplier = 5f,
        OnlyGenerateOneAmbushMonsterPerEncounter = false,
        ShouldDisableCollider = false,
        TimePointWhenYouCanStartParrying = 0f,
        AdditionalAmbushTime = 0f,
        AmbushTeleportHeightIncrease = 0f,
        CameraZoomBackDuration = 0f,
        ObjectToSpawnBeneathMonster = null,
        ObjectToSpawnBeneathMonsterOffset = new(0, 0, 0),
        ShouldTheObjectDisappearAfterCombat = true,
        FollowerAbility = null,
    };

    private static readonly MonsterStatsBuilder Stats = new(
        BaseMaxHealth: 100,
        PerkInfosList: [new(628, 1), new(871, 1), new(1343, 1)]
    );

    private static readonly SkillManagerBuilder SkillManager = new(
        MainType: EMonsterMainType.Hybrid,
        SignatureTrait: new(WheelSupremacy.Trait.ID),
        MonsterTypes: [EMonsterType.Heal, EMonsterType.Regeneration, EMonsterType.Terror],
        Elements: [EElement.Fire, EElement.Water],
        StaggerDefines:
        [
            new() { Element = EElement.Earth, Hits = 15 },
            new() { Element = EElement.Water, Hits = 15 },
        ],
        StartActions: [new(ManyEyed.Action.ID), new(FountainOfLife.Action.ID)],
        EliteTrait: new("Blind"),
        BossStagger:
        [
            new() { Element = EElement.Earth, Hits = 15 },
            new() { Element = EElement.Wind, Hits = 15 },
        ],
        BossAlternativeStagger: null,
        ImpossibleToStagger: false,
        AllAetherDefaultAttack: false
    );

    private static readonly MonsterAIBuilder AI = new(
        ResetAction: null,
        CannotUseDefaultAttack: false,
        ExcludedFromTurnOrder: false,
        Traits: [new("Auto Heal", EDifficulty.Heroic)],
        Scripting:
        [
            new(
                FountainOfLife.Action.ID,
                [
                    new()
                    {
                        Condition = MonsterAIActionCondition.ECondition.HealthBelowPercent,
                        Value = 0.8f,
                    },
                ],
                false
            ),
            new(
                "Foreboding Rain",
                [
                    new()
                    {
                        Condition = MonsterAIActionCondition.ECondition.HealthBelowPercent,
                        Value = 0.9f,
                    },
                ],
                false
            ),
            new(TwistedGarden.Action.ID, [], false),
            new(ManyEyed.Action.ID, [], false),
        ],
        VoidPerks: [],
        VoidPerksTier2: [],
        VoidPerksTier3: [],
        ChampionPerks: [new(628, 2), new(871, 2), new(1343, 2), new(614, 2)]
    );

    private static readonly MonsterShiftBuilder Shift = new()
    {
        Health = 120,
        MonsterTypes = [EMonsterType.Aether, EMonsterType.Affliction, EMonsterType.Age],
        Elements = [EElement.Wild, EElement.Earth],
        SignatureTrait = new("Schadenfreude"),
    };

    internal static readonly MetaUpgradeBuilder MetaUpgrade = new(
        ID: MetaUpgradeID,
        Name: Name,
        Description: "",
        Sidenote: "",
        Cost: 100,
        CostCurrency: ECollectibleLoot.AetherCrystals,
        UpgradeType: EMetaUpgradeType.UnlockMonster,
        UnlockedMonster: new(Monster)
    );

    internal static readonly MonsterMementoBuilder Memento = new(
        ID: MementoID,
        Name: $"{Name} Memento",
        Monster: new(Monster),
        Area: EArea.PilgrimagePath,
        Shift: EMonsterShift.Normal,
        ActionIconBig: new(),
        ActionIconSmall: new(),
        Icon: Sprites.LoadFromImage(
            Sprites.SpriteType.Memento,
            Path.Join(Plugin.CustomMonstersPath, "Memento_WaterWheel.png")
        )
    );

    internal static readonly MonsterMementoBuilder MementoShifted = Memento with
    {
        ID = MementoShiftedID,
        Shift = EMonsterShift.Shifted,
    };
}
