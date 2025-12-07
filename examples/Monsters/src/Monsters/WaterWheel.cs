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

    private static readonly AssetBundle Bundle = Assets.LoadBundle(
        Path.Join(Plugin.CustomMonstersPath, "WaterWheel")
    );

    private static readonly AnimationClip CombatIdle = Animations.LoadFromBundle(
        Bundle,
        "assets/animations/waterwheel/waterwheel_combatidle.prefab"
    );

    private static readonly AnimationClip CombatCast = Animations.LoadFromBundle(
        Bundle,
        "assets/animations/waterwheel/waterwheel_cast.prefab"
    );

    private static readonly AnimationClip CombatCastEnd = Animations.LoadFromBundle(
        Bundle,
        "assets/animations/waterwheel/waterwheel_castend.prefab"
    );

    private static readonly Texture2D Texture = Sprites
        .LoadFromBundle(Bundle, "assets/animations/waterwheel/waterwheel_combatidle.prefab")
        .texture;

    private static readonly Sprite[] ShiftedSprites = Sprites.LoadAllFromBundle(Bundle);

    private static readonly Texture2D ExplorationSpritesheet = new(0, 0);

    internal static MonsterBuilder Builder =>
        new(
            Monster,
            Animator,
            Bounds,
            SkillManager,
            Stats,
            AI,
            OverworldBehaviour,
            Shift,
            ShiftedSprites
        );

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
        Perks: [new(628, 1), new(871, 1), new(1343, 1)]
    );

    private static readonly SkillManagerBuilder SkillManager = new(
        MainType: EMonsterMainType.Hybrid,
        SignatureTrait: new(WheelSupremacy.Trait.ID),
        Types: [EMonsterType.Heal, EMonsterType.Regeneration, EMonsterType.Terror],
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
        Traits:
        [
            new("Auto Heal", EDifficulty.Heroic, EMonsterShift.Normal),
            new("Outlast", EDifficulty.Heroic, EMonsterShift.Shifted),
        ],
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
                ]
            ),
            new(
                "Foreboding Rain",
                [
                    new()
                    {
                        Condition = MonsterAIActionCondition.ECondition.HealthBelowPercent,
                        Value = 0.9f,
                    },
                    new()
                    {
                        Condition = MonsterAIActionCondition.ECondition.MonsterShift,
                        MonsterShift = EMonsterShift.Normal,
                    },
                ]
            ),
            new(
                TwistedGarden.Action.ID,
                [
                    new()
                    {
                        Condition = MonsterAIActionCondition.ECondition.MonsterShift,
                        MonsterShift = EMonsterShift.Normal,
                    },
                ]
            ),
            new(
                "Mud Tide",
                [
                    new()
                    {
                        Condition = MonsterAIActionCondition.ECondition.MonsterShift,
                        MonsterShift = EMonsterShift.Shifted,
                    },
                ]
            ),
            new(
                "Crystal Charge",
                [
                    new()
                    {
                        Condition = MonsterAIActionCondition.ECondition.MonsterShift,
                        MonsterShift = EMonsterShift.Shifted,
                    },
                ]
            ),
            new(ManyEyed.Action.ID, []),
        ],
        VoidPerks: [],
        VoidPerksTier2: [],
        VoidPerksTier3: [],
        ChampionPerks: [new(628, 1), new(871, 2), new(1343, 2), new(614, 2)]
    );

    private static readonly MonsterShiftBuilder Shift = new()
    {
        Health = 120,
        Types = [EMonsterType.Heal, EMonsterType.Age, EMonsterType.Terror],
        Elements = [EElement.Earth, EElement.Water],
        Perks = [new(628, 1), new(1356, 1), new(1343, 1)],
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
