using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ethereal.API;
using HarmonyLib;
using Newtonsoft.Json;

namespace Randomizer.API;

internal class Randomizer
{
    internal const string DataFilePrefix = "data_";

    internal static string CurrentDataPath
    {
        get
        {
            return Path.Combine(
                Plugin.RandomizerPath,
                DataFilePrefix
                    + (string)
                        AccessTools
                            .Field(typeof(SaveFileManager), "selectedProfileID")
                            .GetValue(SaveFileManager.instance)
                    + ".json"
            );
        }
    }

    /// <summary>
    /// Randomize all monster data.
    /// </summary>
    internal static void Randomize()
    {
        Dictionary<int, Monsters.MonsterDescriptor> randomized = [];
        List<int> usedSignatureTraits = [];

        foreach (var gameObject in GameController.Instance.ActiveMonsterList)
        {
            Monster monster = gameObject.GetComponent<Monster>();

            (EElement, EElement) elements = Random.GetRandomUniqueElements();
            List<EMonsterType> types = Random.GetRandomTypes();

            Monsters.MonsterDescriptor descriptor = new()
            {
                Elements = elements,
                Types = types,
                MainType = Random.GetRandomMainType(),
                Perks = Random.GetRandomPerks(),
                Scripting = Random.GetRandomScripting(elements, types, monster.Name == "Mephisto"),
                WildTraits =
                [
                    new()
                    {
                        Trait = Random.GetRandomTrait(types, true).gameObject,
                        MinDifficulty = EDifficulty.Heroic,
                    },
                ],
                EliteTrait = Random.GetRandomTrait(types, true),
                StartingActions = Random.GetRandomStartingActions(elements, types),
                SignatureTrait = Random.GetRandomTrait(types, false),
            };

            while (usedSignatureTraits.Contains(descriptor.SignatureTrait.ID))
                descriptor.SignatureTrait = Random.GetRandomTrait(types, false);

            usedSignatureTraits.Add(descriptor.SignatureTrait.ID);

            randomized.Add(monster.MonID, descriptor);
        }

        foreach ((int id, Monsters.MonsterDescriptor descriptor) in randomized)
            Monsters.Update(id, descriptor);

        SaveData(randomized);
    }

    /// <summary>
    /// Save the randomized monster data to a file.
    /// </summary>
    /// <param name="data"></param>
    private static void SaveData(Dictionary<int, Monsters.MonsterDescriptor> data)
    {
        Dictionary<int, SerializableDescriptor> list = data.Select(x =>
                (id: x.Key, serialized: new SerializableDescriptor(x.Value))
            )
            .ToDictionary(x => x.id, x => x.serialized);

        string json = JsonConvert.SerializeObject(list);

        Directory.CreateDirectory(Plugin.RandomizerPath);
        File.WriteAllText(CurrentDataPath, json);
    }

    /// <summary>
    /// Load the randomized monster data from a file.
    /// </summary>
    internal static void LoadData()
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

        Dictionary<int, Monsters.MonsterDescriptor> data = JsonConvert
            .DeserializeObject<Dictionary<int, SerializableDescriptor>>(json)
            .ToDictionary(x => x.Key, x => x.Value.Deserialize());

        foreach ((int id, Monsters.MonsterDescriptor descriptor) in data)
            Monsters.Update(id, descriptor);
    }

    /// <summary>
    /// Balance a few skills that would be too power in randomizer,
    /// as they appear a lot more often and would overly benefit the player.
    /// </summary>
    internal static void BalanceChanges()
    {
        foreach (var name in (string[])["Fire Bolt", "Water Bolt", "Wind Bolt"])
        {
            // Bolt skills only consume 1 aether for 9 damage,
            // +4 damage and +1 stagger per additional aether
            if (Actions.TryGet(name, out var bolt))
            {
                bolt.GetComponent<ActionDamage>().Damage = 7;
                bolt.GetComponent<ActionDamage>().AdditionalDamage = 2;
            }
        }

        // Lava Bolt consumes 2 aether for 13 damage,
        // +2 burn and +1 stagger per fire aether
        if (Actions.TryGet("Lava Bolt", out var lavaBolt))
            lavaBolt.GetComponent<ActionDamage>().Damage = 10;
    }
}
