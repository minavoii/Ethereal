using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using UnityEngine;

namespace Ethereal.API;

[Deferrable]
public static partial class Encounters
{
    /// <summary>
    /// A helper class that describes an encounter set's properties.
    /// </summary>
    public class EncounterSetDescriptor
    {
        public int EnemyLevel { get; set; }

        public int CasualLevel { get; set; }
        public List<EncounterDescriptor> Encounters { get; set; } = [];
    }

    /// <summary>
    /// A helper class that describes an encounter's properties.
    /// </summary>
    public class EncounterDescriptor
    {
        public List<string> Enemies { get; set; } = [];
        public MonsterEncounter.EEncounterType Type { get; set; }

        public MonsterEncounter ToEncounter()
        {
            List<GameObject> monsters = [];
            foreach (string enemy in Enemies)
            {
                Monsters.TryGet(enemy, out Monster monster);
                if (monster != null)
                {
                    monsters.Add(monster.gameObject);
                }
            }

            return new MonsterEncounter()
            {
                EncounterType = Type,
                Enemies = monsters
            };
        }
    }

    /// <summary>
    /// Get an encounter set by name.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static MonsterEncounterSet? Get(string name) => Get(x => x?.name == name);

    private static MonsterEncounterSet? Get(Func<MonsterEncounterSet?, bool> predicate) =>
        Resources.FindObjectsOfTypeAll<MonsterEncounterSet>()
            .Where(predicate)
            .FirstOrDefault();

    /// <summary>
    /// Get all encounter sets.
    /// </summary>
    /// <returns></returns>
    [TryGet]
    private static List<MonsterEncounterSet> GetAll() =>
        [
            .. Resources.FindObjectsOfTypeAll<MonsterEncounterSet>()
        ];

    /// <summary>
    /// Overwrite an encounter set's properties with values from a descriptor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="descriptor"></param>
    [Deferrable]
    private static void Update_Impl(string name, EncounterSetDescriptor descriptor)
    {
        if (TryGet(name, out var encounterSet))
            Update(encounterSet, descriptor);
    }

    /// <summary>
    /// Overwrite an encounter set's properties with values from a descriptor.
    /// </summary>
    /// <param name="encounterSet"></param>
    /// <param name="descriptor"></param>
    private static void Update(MonsterEncounterSet encounterSet, EncounterSetDescriptor descriptor)
    {
        if (descriptor.EnemyLevel > 0)
            encounterSet.EnemyLevel = descriptor.EnemyLevel;

        if (descriptor.CasualLevel > 0)
            encounterSet.CasualLevel = descriptor.CasualLevel;

        if (descriptor.Encounters.Count > 0)
            encounterSet.MonsterEncounters = descriptor.Encounters.Select(e => e.ToEncounter()).ToList();
    }
}
