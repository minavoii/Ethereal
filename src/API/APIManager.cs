using System;
using System.Threading.Tasks;

namespace Ethereal.API;

[Flags]
public enum EtherealAPI
{
    Actions = 1 << 0,
    Artifacts = 1 << 1,
    Biomes = 1 << 2,
    Buffs = 1 << 3,
    Encounters = 1 << 4,
    Elements = 1 << 5,
    Equipments = 1 << 6,
    Fonts = 1 << 7,
    Keywords = 1 << 8,
    Localisation = 1 << 9,
    Mementos = 1 << 10,
    MetaUpgrades = 1 << 11,
    Monsters = 1 << 12,
    MonsterTypes = 1 << 13,
    Perks = 1 << 14,
    Referenceables = 1 << 15,
    Sprites = 1 << 16,
    Traits = 1 << 17,
}

public static partial class APIManager
{
    /// <summary>
    /// Wait for the specified API(s) to be ready to run an action,
    /// or immediately if no waiting is needed.
    /// </summary>
    /// <param name="apis"></param>
    /// <param name="action"></param>
    public static async void RunWhenReady(EtherealAPI apis, Action action)
    {
        await WhenReady(apis);
        action();
    }

    /// <summary>
    /// Wait for the specified API(s) to be ready,
    /// or return immediately if no waiting is needed.
    /// </summary>
    /// <param name="apis"></param>
    /// <returns></returns>
    public static async Task WhenReady(EtherealAPI apis)
    {
        if (apis.HasFlag(EtherealAPI.Actions))
            await Actions.WhenReady();

        if (apis.HasFlag(EtherealAPI.Artifacts))
            await Artifacts.WhenReady();

        if (apis.HasFlag(EtherealAPI.Biomes))
            await Biomes.WhenReady();

        if (apis.HasFlag(EtherealAPI.Buffs))
            await Buffs.WhenReady();

        if (apis.HasFlag(EtherealAPI.Encounters))
            await Encounters.WhenReady();

        if (apis.HasFlag(EtherealAPI.Elements))
            await Elements.WhenReady();

        if (apis.HasFlag(EtherealAPI.Equipments))
            await Equipments.WhenReady();

        if (apis.HasFlag(EtherealAPI.Fonts))
            await Fonts.WhenReady();

        if (apis.HasFlag(EtherealAPI.Keywords))
            await Keywords.WhenReady();

        if (apis.HasFlag(EtherealAPI.Localisation))
            await Localisation.WhenReady();

        if (apis.HasFlag(EtherealAPI.Mementos))
            await Mementos.WhenReady();

        if (apis.HasFlag(EtherealAPI.MetaUpgrades))
            await MetaUpgrades.WhenReady();

        if (apis.HasFlag(EtherealAPI.Monsters))
            await Monsters.WhenReady();

        if (apis.HasFlag(EtherealAPI.MonsterTypes))
            await MonsterTypes.WhenReady();

        if (apis.HasFlag(EtherealAPI.Perks))
            await Perks.WhenReady();

        if (apis.HasFlag(EtherealAPI.Referenceables))
            await Referenceables.WhenReady();

        if (apis.HasFlag(EtherealAPI.Sprites))
            await Sprites.WhenReady();

        if (apis.HasFlag(EtherealAPI.Traits))
            await Traits.WhenReady();
    }
}
