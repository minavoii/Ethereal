using System;
using System.Threading.Tasks;

namespace Ethereal.API;

[Flags]
public enum EtherealAPI
{
    Actions = 1 << 0,
    Artifacts = 1 << 1,
    Buffs = 1 << 2,
    Elements = 1 << 3,
    Equipments = 1 << 4,
    Fonts = 1 << 5,
    Localisation = 1 << 6,
    Mementos = 1 << 7,
    MetaUpgrades = 1 << 8,
    Monsters = 1 << 9,
    MonsterTypes = 1 << 10,
    Perks = 1 << 11,
    Referenceables = 1 << 12,
    Sprites = 1 << 13,
    Traits = 1 << 14,
    Keywords = 1 << 15,
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
        await WaitUntilReady(apis);
        action();
    }

    /// <summary>
    /// Wait for the specified API(s) to be ready,
    /// or return immediately if no waiting is needed.
    /// </summary>
    /// <param name="apis"></param>
    /// <returns></returns>
    public static async Task WaitUntilReady(EtherealAPI apis)
    {
        if (apis.HasFlag(EtherealAPI.Actions))
            await Actions.Task;

        if (apis.HasFlag(EtherealAPI.Artifacts))
            await Artifacts.Task;

        if (apis.HasFlag(EtherealAPI.Buffs))
            await Buffs.Task;

        if (apis.HasFlag(EtherealAPI.Elements))
            await Elements.Task;

        if (apis.HasFlag(EtherealAPI.Equipments))
            await Equipments.Task;

        if (apis.HasFlag(EtherealAPI.Fonts))
            await Fonts.Task;

        if (apis.HasFlag(EtherealAPI.Localisation))
            await Localisation.Task;

        if (apis.HasFlag(EtherealAPI.Mementos))
            await Mementos.Task;

        if (apis.HasFlag(EtherealAPI.MetaUpgrades))
            await MetaUpgrades.Task;

        if (apis.HasFlag(EtherealAPI.Monsters))
            await Monsters.Task;

        if (apis.HasFlag(EtherealAPI.MonsterTypes))
            await MonsterTypes.Task;

        if (apis.HasFlag(EtherealAPI.Perks))
            await Perks.Task;

        if (apis.HasFlag(EtherealAPI.Referenceables))
            await Referenceables.Task;

        if (apis.HasFlag(EtherealAPI.Sprites))
            await Sprites.Task;

        if (apis.HasFlag(EtherealAPI.Traits))
            await Traits.Task;

        if (apis.HasFlag(EtherealAPI.Keywords))
            await Keywords.Task;
    }
}
