using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ethereal.API;
using Ethereal.Classes.Builders;
using Ethereal.Classes.LazyValues;
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
                            .GetValue(SaveFileManager.Instance)
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

        if (Monsters.TryGetAll(out List<Monster> monsters))
        {
            foreach (Monster monster in monsters)
            {
                (EElement, EElement) elements = Random.GetRandomUniqueElements();
                List<EMonsterType> types = Random.GetRandomTypes();
                Trait signatureTrait = Random.GetRandomTrait(types, false, usedSignatureTraits);

                Monsters.MonsterDescriptor descriptor = new()
                {
                    Elements = elements,
                    Types = types,
                    MainType = Random.GetRandomMainType(),
                    Perks = [.. Random.GetRandomPerks().Select(x => new PerkInfosBuilder(x))],
                    Scripting =
                    [
                        .. Random
                            .GetRandomScripting(elements, types, monster.Name == "Mephisto")
                            .Select(x => new MonsterAIActionBuilder(
                                x.Action.GetComponent<BaseAction>(),
                                x.Conditions,
                                x.IsTemporary
                            )),
                    ],
                    WildTraits = [new(Random.GetRandomTrait(types, true), EDifficulty.Heroic)],
                    EliteTrait = new(Random.GetRandomTrait(types, true)),
                    StartingActions =
                    [
                        .. Random
                            .GetRandomStartingActions(elements, types)
                            .Select(x => new LazyAction(x)),
                    ],
                    SignatureTrait = new(signatureTrait),
                };

                usedSignatureTraits.Add(signatureTrait.ID);
                randomized.Add(monster.ID, descriptor);
            }

            foreach ((int id, Monsters.MonsterDescriptor descriptor) in randomized)
                Monsters.Update(id, descriptor);
        }

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
        foreach (string name in (string[])["Fire Bolt", "Water Bolt", "Wind Bolt"])
        {
            // Bolt skills only consume 1 aether for 9 damage,
            // +4 damage and +1 stagger per additional aether
            if (Actions.TryGet(name, out BaseAction bolt))
            {
                bolt.GetComponent<ActionDamage>().Damage = 7;
                bolt.GetComponent<ActionDamage>().AdditionalDamage = 2;
            }
        }

        // Lava Bolt consumes 2 aether for 13 damage,
        // +2 burn and +1 stagger per fire aether
        if (Actions.TryGet("Lava Bolt", out BaseAction lavaBolt))
            lavaBolt.GetComponent<ActionDamage>().Damage = 10;
    }
}
