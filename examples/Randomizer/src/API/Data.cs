using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.API;
using Ethereal.Utils.Extensions;

namespace Randomizer.API;

internal static class Data
{
    internal static List<PerkInfos> AllPerks = [];

    internal static List<Trait> SignatureTraits = [];

    internal static Dictionary<string, List<List<int>>> VanillaEncounters = [];

    /// <summary>
    /// Get all buffs related to the selected monster types. <br/>
    /// Only include buffs derived from this monster's type.
    /// </summary>
    /// <param name="types"></param>
    /// <returns></returns>
    internal static async Task<List<Buff>> GetTypeBuffs(List<EMonsterType> types) =>
        [
            .. (await types.SelectAsync(x => (Task<Buff>)Buffs.Get(x.ToString())!))
                .Where(x => x is not null)
                .Distinct(),
        ];

    internal static async Task<List<PerkInfos>> GetAllPerkInfos() =>
        [
            .. (await Monsters.GetAll()).SelectMany(x =>
                x?.GetComponent<MonsterStats>()?.PerkInfosList
            ),
        ];

    internal static async Task<Dictionary<string, List<List<int>>>> GetEncounters()
    {
        List<MonsterEncounterSet> encounters = await Encounters.GetAll();

        Dictionary<string, List<List<int>>> data = [];

        foreach (MonsterEncounterSet set in encounters)
        {
            List<List<int>> ints =
            [
                .. set.MonsterEncounters.Select(encounter =>
                    (List<int>)
                        [
                            .. encounter
                                .Enemies.Select(monster => monster?.GetComponent<Monster>()?.ID)
                                .Where(monster => monster is not null)
                                .OfType<int>(),
                        ]
                ),
            ];

            data.Add(set.name, ints);
        }

        return data;
    }
}
