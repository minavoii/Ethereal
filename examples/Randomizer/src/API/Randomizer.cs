using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.View;
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

        SaveData(randomized);
    }

    /// <summary>
    /// Save the randomized monster data to a file.
    /// </summary>
    /// <param name="data"></param>
    private static void SaveData(Dictionary<int, MonsterView> data)
    {
        Dictionary<int, SerializableView> list = data.ToDictionary(
            x => x.Key,
            x => new SerializableView(x.Value)
        );

        string json = JsonConvert.SerializeObject(list);

        Directory.CreateDirectory(Plugin.RandomizerPath);
        File.WriteAllText(CurrentDataPath, json);
    }

    /// <summary>
    /// Load the randomized monster data from a file.
    /// </summary>
    internal static async Task LoadData()
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

        await Task.WhenAll(
            JsonConvert
                .DeserializeObject<Dictionary<int, SerializableView>>(json)
                .Select(async x => await x.Value.Deserialize())
        );
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
            ActionDamage damage = (await Actions.Get(name)).GetComponent<ActionDamage>();
            damage.Damage = 7;
            damage.AdditionalDamage = 2;
        }

        // Lava Bolt consumes 2 aether for 13 damage,
        // +2 burn and +1 stagger per fire aether
        (await Actions.Get("Lava Bolt"))
            .GetComponent<ActionDamage>()
            .Damage = 10;
    }
}
