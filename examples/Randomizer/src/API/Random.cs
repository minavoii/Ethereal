using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.API;
using static EActionSubType;
using static MonsterAIActionCondition.ECondition;

namespace Randomizer.API;

internal class Random
{
    private enum BoolType
    {
        False,
        True,
        Either,
    }

    /// <summary>
    /// Get two random and different elements.
    /// </summary>
    /// <returns></returns>
    internal static (EElement, EElement) GetRandomUniqueElements()
    {
        List<int> elements = GetUniqueRandomNumbers(0, 4, 2);
        return ((EElement)elements[0], (EElement)elements[1]);
    }

    /// <summary>
    /// Get a list of random and different monster types.
    /// </summary>
    /// <returns></returns>
    internal static List<EMonsterType> GetRandomTypes()
    {
        return
        [
            .. GetUniqueRandomNumbers(0, 16, 3)
                // Some types are currently still unused
                .Select(x =>
                    x == (int)EMonsterType.SignatureTraits ? EMonsterType.Terror
                    : x == (int)EMonsterType.StartingActions ? EMonsterType.Weakness
                    : (EMonsterType)x
                ),
        ];
    }

    /// <summary>
    /// Get a list of random and different main monster types.
    /// </summary>
    /// <returns></returns>
    internal static EMonsterMainType GetRandomMainType()
    {
        return (EMonsterMainType)Shuffle(0, 3)[0];
    }

    /// <summary>
    /// Get a list of random and different perks.
    /// </summary>
    /// <returns></returns>
    internal static List<PerkInfos> GetRandomPerks()
    {
        int[] perks = Shuffle(0, Data.AllPerks.Count);

        return [Data.AllPerks[perks[0]], Data.AllPerks[perks[1]], Data.AllPerks[perks[2]]];
    }

    /// <summary>
    /// Get a list of random and different traits.
    /// </summary>
    /// <param name="types"></param>
    /// <param name="allowMaverick"></param>
    /// <returns></returns>
    internal static Trait GetRandomTrait(List<EMonsterType> types, bool allowMaverick)
    {
        List<Trait> traits =
        [
            .. types
                .SelectMany(x => MonsterTypes.NativeTypes[x].GetComponent<MonsterType>().Traits)
                .Concat(Data.SignatureTraits)
                .Distinct()
                .Where(x => allowMaverick || x.MaverickSkill == false),
        ];

        int[] indexes = Shuffle(0, traits.Count);

        return traits[indexes[0]];
    }

    /// <summary>
    /// Get conditions for a monster AI's scripted action.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    private static List<MonsterAIActionCondition> GetActionConditions(BaseAction action)
    {
        List<MonsterAIActionCondition> conditions = [];
        List<EActionSubType> subTypes = [];

        // Exception with Boiling Soup's `NonCookingConsumingAction` in `GetSubTypes()`
        try
        {
            subTypes = action.GetSubTypes();
        }
        catch (NullReferenceException)
        {
            subTypes = [DamagingAction, DebuffAction];
        }

        // Healing actions: only once at <= 50% HP
        if (subTypes.Any(x => x == HealingAction))
            conditions =
            [
                new() { Condition = HealthBelowPercent, Value = .5f },
                new() { Condition = UseOnce },
            ];
        // Shielding actions: only once at <= 70% HP
        else if (subTypes.Any(x => x == ShieldingAction))
            conditions =
            [
                new() { Condition = HealthBelowPercent, Value = .75f },
                new() { Condition = UseOnce },
            ];
        // Maverick actions: only once
        else if (action.MaverickSkill)
            conditions = [new() { Condition = UseOnce }];

        return conditions;
    }

    /// <summary>
    /// Get random scripted actions.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="types"></param>
    /// <param name="skipConditions"></param>
    /// <returns></returns>
    internal static List<MonsterAIAction> GetRandomScripting(
        (EElement first, EElement second) elements,
        List<EMonsterType> types,
        bool skipConditions
    )
    {
        List<MonsterAIAction> script = [];

        var first = GetRandomAction1(elements, types);
        var second = GetRandomAction2(elements, types, BoolType.Either, first);
        var third = GetRandomAction3(elements, types);

        if (skipConditions)
        {
            script.Add(new() { Action = third.gameObject, Conditions = [] });
            script.Add(new() { Action = second.gameObject, Conditions = [] });
            script.Add(new() { Action = first.gameObject, Conditions = [] });

            return script;
        }

        List<MonsterAIActionCondition> conditions3 =
        [
            new() { Condition = BiomeTierEqualAbove, Value = 2f },
            new() { Condition = UseOnce },
        ];

        script.Add(new() { Action = third.gameObject, Conditions = conditions3 });
        script.Add(new() { Action = second.gameObject, Conditions = GetActionConditions(second) });
        script.Add(new() { Action = first.gameObject, Conditions = GetActionConditions(first) });

        return script;
    }

    /// <summary>
    /// Get random and different starting actions.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    internal static List<BaseAction> GetRandomStartingActions(
        (EElement first, EElement second) elements,
        List<EMonsterType> types
    )
    {
        BaseAction first = GetRandomAction1(elements, types);
        BaseAction second = GetRandomAction2(elements, types, BoolType.False, first);

        return [first, second];
    }

    /// <summary>
    /// Get a random action based on critera.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="types"></param>
    /// <param name="excludeAction"></param>
    /// <param name="byType"></param>
    /// <param name="byElements"></param>
    /// <param name="costMinInclusive"></param>
    /// <param name="costMaxExclusive"></param>
    /// <param name="forceHealOrShield"></param>
    /// <param name="onlyPossibleBuffs"></param>
    /// <param name="maverick"></param>
    /// <param name="freeAction"></param>
    /// <returns></returns>
    private static BaseAction GetRandomAction(
        (EElement first, EElement second) elements,
        List<EMonsterType> types,
        BaseAction excludeAction,
        bool byType,
        bool byElements,
        int costMinInclusive,
        int costMaxExclusive,
        bool forceHealOrShield,
        bool onlyPossibleBuffs,
        BoolType maverick,
        BoolType freeAction
    )
    {
        List<Buff> possibleBuffs = Data.GetTypeBuffs(types);

        List<BaseAction> dataset = byType
            ?
            [
                .. types
                    .SelectMany(x =>
                        MonsterTypes.NativeTypes[x]?.GetComponent<MonsterType>().Actions
                    )
                    .Distinct(),
            ]
            : [.. Data.GetAllActions()];

        List<BaseAction> actions =
        [
            .. dataset.Where(x =>
                x.Cost.GetAll() >= costMinInclusive
                && x.Cost.GetAll() < costMaxExclusive
                && (!byElements || x.Elements.All(x => x == elements.first || x == elements.second))
                && (
                    !forceHealOrShield
                    || x.IsActionSubType(
                        types.Contains(EMonsterType.Heal) && types.Contains(EMonsterType.Shield)
                            ? HealingOrShieldingAction
                        : types.Contains(EMonsterType.Heal) ? HealingAction
                        : types.Contains(EMonsterType.Shield) ? ShieldingAction
                        : DamagingAction
                    )
                )
                && (
                    !onlyPossibleBuffs
                    || x.AppliedBuffs.All(applied =>
                        possibleBuffs.Any(possible =>
                            possible.ID == applied.GetComponent<Buff>().ID
                        )
                    )
                )
                && (excludeAction == null || x.ID != excludeAction.ID)
                && (
                    (maverick == BoolType.False && x.MaverickSkill == false)
                    || (maverick == BoolType.True && x.MaverickSkill == true)
                    || (maverick == BoolType.Either)
                )
                && (
                    (freeAction == BoolType.False && x.FreeAction == false)
                    || (freeAction == BoolType.True && x.FreeAction == true)
                    || (freeAction == BoolType.Either)
                )
            ),
        ];

        return actions.Count == 0 ? null : actions[Shuffle(0, actions.Count)[0]];
    }

    /// <summary>
    /// Get the first random action. <para/>
    /// It should cost 1 aether and be of the same element as the monster.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    private static BaseAction GetRandomAction1(
        (EElement first, EElement second) elements,
        List<EMonsterType> types
    )
    {
        return GetRandomAction(
            elements,
            types,
            null,
            false,
            true,
            1,
            2,
            false,
            true,
            BoolType.False,
            BoolType.Either
        );
    }

    /// <summary>
    /// Get the second random action. <para/>
    /// It should cost 1 or 2 aether and ideally heal or shield if it's
    /// a healing or shielding action, otherwise a damaging action. <para/>
    /// It should be based on the monster's types and/or elements,
    /// whichever is available first.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="types"></param>
    /// <param name="maverick"></param>
    /// <param name="excludeAction"></param>
    /// <returns></returns>
    private static BaseAction GetRandomAction2(
        (EElement first, EElement second) elements,
        List<EMonsterType> types,
        BoolType maverick,
        BaseAction excludeAction
    )
    {
        return GetRandomAction(
                elements,
                types,
                excludeAction,
                true,
                true,
                1,
                3,
                true,
                true,
                maverick,
                BoolType.False
            )
            ?? GetRandomAction(
                elements,
                types,
                excludeAction,
                true,
                false,
                1,
                3,
                true,
                true,
                maverick,
                BoolType.False
            )
            ?? GetRandomAction(
                elements,
                types,
                excludeAction,
                false,
                true,
                2,
                3,
                true,
                false,
                maverick,
                BoolType.False
            );
    }

    /// <summary>
    /// Get the third random action. <para/>
    /// It should cost at least 3 aether, be a maverick action, and
    /// be based on the monster's types or elements, whichever is available first.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    private static BaseAction GetRandomAction3(
        (EElement first, EElement second) elements,
        List<EMonsterType> types
    )
    {
        return GetRandomAction(
                elements,
                types,
                null,
                true,
                false,
                3,
                6,
                true,
                true,
                BoolType.True,
                BoolType.False
            )
            ?? GetRandomAction(
                elements,
                types,
                null,
                false,
                true,
                3,
                6,
                false,
                true,
                BoolType.True,
                BoolType.False
            );
    }

    /// <summary>
    /// Get random and different numbers.
    /// </summary>
    /// <param name="minInclusive"></param>
    /// <param name="maxExclusive"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    private static List<int> GetUniqueRandomNumbers(int minInclusive, int maxExclusive, int amount)
    {
        System.Random random = new();
        List<int> result = [];

        for (int i = 0; i < amount; i++)
        {
            int value = random.Next(minInclusive, maxExclusive);

            while (result.Contains(value))
                value = random.Next(minInclusive, maxExclusive);

            result.Add(value);
        }

        return result;
    }

    /// <summary>
    /// Shuffle a number range array between a minimum and a maximum.
    /// </summary>
    /// <param name="minInclusive"></param>
    /// <param name="maxExclusive"></param>
    /// <returns></returns>
    private static int[] Shuffle(int minInclusive, int maxExclusive)
    {
        System.Random rnd = new();
        int[] arr = [.. Enumerable.Range(minInclusive, maxExclusive)];

        for (int i = arr.Length; i > 1; i--)
        {
            int pos = rnd.Next(i);
            (arr[pos], arr[i - 1]) = (arr[i - 1], arr[pos]);
        }

        return arr;
    }
}
