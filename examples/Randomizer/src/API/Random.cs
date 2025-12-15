using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.API;
using Ethereal.Utils.Extensions;
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
    internal static List<EElement> GetRandomUniqueElements() =>
        [.. GetUniqueRandomNumbers(0, 4, 2).Select(x => (EElement)x)];

    /// <summary>
    /// Get a list of random and different monster types.
    /// </summary>
    /// <returns></returns>
    internal static List<EMonsterType> GetRandomTypes() =>
        [
            .. GetUniqueRandomNumbers(0, 18, 3)
                // Some types are currently still unused
                .Select(x =>
                    x == (int)EMonsterType.SignatureTraits ? EMonsterType.Weakness
                    : x == (int)EMonsterType.StartingActions ? EMonsterType.Force
                    : x == (int)EMonsterType.Unknown ? EMonsterType.Summon
                    : (EMonsterType)x
                ),
        ];

    /// <summary>
    /// Get a list of random and different main monster types.
    /// </summary>
    /// <returns></returns>
    internal static EMonsterMainType GetRandomMainType() => (EMonsterMainType)Shuffle(0, 3)[0];

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
    /// <param name="excludeIDs">A list of Trait IDs to exclude.</param>
    /// <returns></returns>
    internal static async Task<Trait> GetRandomTrait(
        List<EMonsterType> types,
        bool allowMaverick,
        List<int>? excludeIDs = null
    )
    {
        excludeIDs ??= [];

        List<Trait> traits =
        [
            .. (await types.SelectManyAsync(async x => (await MonsterTypes.Get(x))?.Traits ?? []))
                .Concat(Data.SignatureTraits)
                .Distinct()
                .Where(x => allowMaverick || x.MaverickSkill == false)
                .Where(x => !excludeIDs.Any(y => y == x.ID)),
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
    internal static async Task<List<MonsterAIAction>> GetRandomScripting(
        List<EElement> elements,
        List<EMonsterType> types,
        bool skipConditions
    )
    {
        List<MonsterAIAction> script = [];

        BaseAction? first = await GetRandomAction1(elements, types);
        BaseAction? second = await GetRandomAction2(elements, types, BoolType.Either, first);
        BaseAction? third = await GetRandomAction3(elements, types);

        if (skipConditions)
        {
            if (third is not null)
                script.Add(new() { Action = third.gameObject, Conditions = [] });

            if (second is not null)
                script.Add(new() { Action = second.gameObject, Conditions = [] });

            if (first is not null)
                script.Add(new() { Action = first.gameObject, Conditions = [] });

            return script;
        }

        List<MonsterAIActionCondition> conditions3 =
        [
            new() { Condition = BiomeTierEqualAbove, Value = 2f },
            new() { Condition = UseOnce },
        ];

        if (third is not null)
            script.Add(new() { Action = third.gameObject, Conditions = conditions3 });

        if (second is not null)
            script.Add(
                new() { Action = second.gameObject, Conditions = GetActionConditions(second) }
            );

        if (first is not null)
            script.Add(
                new() { Action = first.gameObject, Conditions = GetActionConditions(first) }
            );

        return script;
    }

    /// <summary>
    /// Get random and different starting actions.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    internal static async Task<List<BaseAction>> GetRandomStartingActions(
        List<EElement> elements,
        List<EMonsterType> types
    )
    {
        List<BaseAction> startActions = [];

        if (await GetRandomAction1(elements, types) is BaseAction first)
        {
            startActions.Add(first);

            if (await GetRandomAction2(elements, types, BoolType.False, first) is BaseAction second)
                startActions.Add(second);
        }

        return startActions;
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
    private static async Task<BaseAction?> GetRandomAction(
        List<EElement> elements,
        List<EMonsterType> types,
        BaseAction? excludeAction,
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
        List<Buff> possibleBuffs = await Data.GetTypeBuffs(types);

        List<BaseAction> dataset = [];

        if (byType)
            dataset =
            [
                .. (
                    await types.SelectManyAsync(async x =>
                        (await MonsterTypes.Get(x))?.Actions ?? []
                    )
                ).Distinct(),
            ];
        else
            dataset = await Actions.GetAll();

        List<BaseAction> actions =
        [
            .. dataset.Where(x =>
                x.Cost.GetAll() >= costMinInclusive
                && x.Cost.GetAll() < costMaxExclusive
                && (!byElements || x.Elements.All(x => x == elements[0] || x == elements[1]))
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
                && (x.ID != excludeAction?.ID)
                && (
                    (maverick == BoolType.False && x.MaverickSkill == false)
                    || (maverick == BoolType.True && x.MaverickSkill == true)
                    || (maverick == BoolType.Either)
                )
                && (
                    (freeAction == BoolType.False && !x.IsFreeAction())
                    || (freeAction == BoolType.True && x.IsFreeAction())
                    || (freeAction == BoolType.Either)
                )
            ),
        ];

        return actions.Count > 0 ? actions[Shuffle(0, actions.Count)[0]] : null;
    }

    /// <summary>
    /// Get the first random action. <para/>
    /// It should cost 1 aether and be of the same element as the monster.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    private static async Task<BaseAction?> GetRandomAction1(
        List<EElement> elements,
        List<EMonsterType> types
    ) =>
        await GetRandomAction(
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
    private static async Task<BaseAction?> GetRandomAction2(
        List<EElement> elements,
        List<EMonsterType> types,
        BoolType maverick,
        BaseAction? excludeAction
    ) =>
        await GetRandomAction(
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
        ?? await GetRandomAction(
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
        ?? await GetRandomAction(
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

    /// <summary>
    /// Get the third random action. <para/>
    /// It should cost at least 3 aether, be a maverick action, and
    /// be based on the monster's types or elements, whichever is available first.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    private static async Task<BaseAction?> GetRandomAction3(
        List<EElement> elements,
        List<EMonsterType> types
    ) =>
        await GetRandomAction(
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
        ?? await GetRandomAction(
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
