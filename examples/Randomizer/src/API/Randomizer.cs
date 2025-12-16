using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.LazyValues;
using Ethereal.Classes.Views;
using Ethereal.Utils.Extensions;
using HarmonyLib;
using Newtonsoft.Json;

namespace Randomizer.API;

internal class Randomizer
{
    internal const string DataFilePrefix = "data_";

    internal static string CurrentDataPath =>
        Path.Combine(
            Plugin.RandomizerPath,
            DataFilePrefix
                + (string)
                    AccessTools
                        .Field(typeof(SaveFileManager), "selectedProfileID")
                        .GetValue(SaveFileManager.Instance)
                + ".json"
        );

    /// <summary>
    /// Randomize all monster data.
    /// </summary>
    internal static async Task Randomize()
    {
        Dictionary<int, MonsterView> randomized = [];
        List<int> usedSignatureTraits = [];

        foreach (Monster monster in await Monsters.GetAll())
        {
            List<EElement> elements = Random.GetRandomUniqueElements();
            List<EMonsterType> types = Random.GetRandomTypes();
            Trait signatureTrait = await Random.GetRandomTrait(types, false, usedSignatureTraits);

            MonsterView view = new(monster)
            {
                Elements = elements,
                Types = types,
                MainType = Random.GetRandomMainType(),
                BasePerks = Random.GetRandomPerks(),
                Scripting = await Random.GetRandomScripting(
                    elements,
                    types,
                    monster.Name == "Mephisto"
                ),
                WildTraits =
                [
                    await new MonsterAITraitBuilder(
                        await Random.GetRandomTrait(types, true),
                        EDifficulty.Heroic
                    ).Build(),
                ],
                EliteTrait = await Random.GetRandomTrait(types, true),
                StartActions = await Random.GetRandomStartingActions(elements, types),
                SignatureTrait = signatureTrait,
            };

            usedSignatureTraits.Add(signatureTrait.ID);
            randomized.Add(monster.ID, view);
        }

        Save(randomized, await RandomizeMapping());
    }

    /// <summary>
    /// Randomize the `original` to `replaced` monster mapping of wild encounters.
    /// </summary>
    /// <returns></returns>
    private static async Task<Dictionary<int, int>> RandomizeMapping()
    {
        Dictionary<int, int> mapping = Random.GetRandomMapping(await Monsters.GetAll());

        await ApplyRandomizedMapping(mapping);
        return mapping;
    }

    /// <summary>
    /// Replace monsters in wild encounters according to the given mapping.
    /// </summary>
    /// <param name="mapping"></param>
    /// <returns></returns>
    private static async Task ApplyRandomizedMapping(Dictionary<int, int> mapping)
    {
        List<MonsterEncounterSet> encounters = await Encounters.GetAll();
        List<Monster> monsters = await Monsters.GetAll();

        foreach (MonsterEncounterSet set in encounters)
        {
            foreach (MonsterEncounter encounter in set.MonsterEncounters)
            {
                // Ignore Chernobog
                if (encounter.Enemies.Any(x => x?.GetComponent<Monster>().MonID == -1))
                    continue;

                await Encounters.SetEnemies(
                    encounter,
                    [
                        .. encounter
                            .Enemies.Select(x =>
                                x?.GetComponent<Monster>() is Monster original
                                    ? new LazyMonster(mapping[original.ID])
                                    : null!
                            )
                            .Where(x => x is not null),
                    ]
                );
            }
        }
    }

    /// <summary>
    /// Reset all wild monster encounters.
    /// </summary>
    /// <returns></returns>
    private static async Task ResetMapping()
    {
        Dictionary<string, List<List<int>>> vanillaEncounters = Data.VanillaEncounters;
        List<MonsterEncounterSet> sets = await Encounters.GetAll();
        List<Monster> monsters = await Monsters.GetAll();

        foreach ((string setName, List<List<int>> setData) in vanillaEncounters)
        {
            MonsterEncounterSet currentSet = sets.Find(x => x.name == setName);

            for (int i = 0; i < setData.Count; i++)
            {
                List<int> enemyData = setData[i];
                MonsterEncounter encounter = currentSet.MonsterEncounters[i];

                encounter.Enemies = [.. await enemyData.SelectAsync(Monsters.GetObject)];
            }
        }
    }

    /// <summary>
    /// Save the randomized monster data to a file.
    /// </summary>
    /// <param name="views"></param>
    private static void Save(Dictionary<int, MonsterView> views, Dictionary<int, int> mapping)
    {
        Dictionary<int, SerializableView> list = views.ToDictionary(
            x => x.Key,
            x => new SerializableView(x.Value)
        );

        SaveData saveData = new(list, mapping);
        string json = JsonConvert.SerializeObject(saveData);

        Directory.CreateDirectory(Plugin.RandomizerPath);
        File.WriteAllText(CurrentDataPath, json);
    }

    /// <summary>
    /// Load the randomized monster data from a file.
    /// </summary>
    internal static async Task Load()
    {
        string json = "";

        try
        {
            json = File.ReadAllText(CurrentDataPath);
        }
        catch
        {
            return;
        }

        if (JsonConvert.DeserializeObject<SaveData>(json) is SaveData saveData)
        {
            await Task.WhenAll(saveData.Views.Select(x => x.Value.Deserialize()));
            await ResetMapping();
            await ApplyRandomizedMapping(saveData.Mapping);
        }
    }

    /// <summary>
    /// Balance a few skills that would be too power in randomizer,
    /// as they appear a lot more often and would overly benefit the player.
    /// </summary>
    internal static async Task BalanceChanges()
    {
        foreach (string name in (string[])["Fire Bolt", "Water Bolt", "Wind Bolt"])
        {
            // Bolt skills only consume 1 aether for 9 damage,
            // +4 damage and +1 stagger per additional aether
            if ((await Actions.Get(name))?.GetComponent<ActionDamage>() is ActionDamage bolt)
            {
                bolt.Damage = 7;
                bolt.AdditionalDamage = 2;
            }
        }

        // Lava Bolt consumes 2 aether for 13 damage,
        // +2 burn and +1 stagger per fire aether
        if ((await Actions.Get("Lava Bolt"))?.GetComponent<ActionDamage>() is ActionDamage lavaBolt)
            lavaBolt.Damage = 10;
    }
}
