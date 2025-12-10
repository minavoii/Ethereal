using System;
using System.Collections.Generic;
using System.Linq;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;

namespace Ethereal.API;

[Deferrable]
public static partial class Artifacts
{
    /// <summary>
    /// Get an artifact by id.
    /// </summary>
    /// <param name="id"></param>
    [TryGet]
    private static Consumable? Get(int id) => Get(x => x?.BaseItem.ID == id);

    /// <summary>
    /// Get an artifact by id.
    /// </summary>
    /// <param name="name"></param>
    [TryGet]
    private static Consumable? Get(string name) => Get(x => x?.BaseItem.Name == name);

    private static Consumable? Get(Predicate<ItemManager.BaseItemInstance?> predicate) =>
        GameController.Instance.ItemManager.Consumables.Find(predicate)?.BaseItem as Consumable;

    [TryGet]
    private static List<Consumable> GetAll() =>
        [
            .. GameController.Instance.ItemManager.Consumables.Select(x =>
                (x.BaseItem as Consumable)!
            ),
        ];

    /// <summary>
    /// Create a new artifact and add it to the game's data
    /// </summary>
    /// <param name="artifact"></param>
    [Deferrable]
    private static void Add_Impl(ArtifactBuilder artifact) => Add_Impl(artifact.Build());

    /// <summary>
    /// Create a new artifact and add it to the game's data
    /// </summary>
    /// <param name="artifact"></param>
    [Deferrable]
    private static void Add_Impl(Consumable artifact)
    {
        GameController.Instance.ItemManager.Consumables.Insert(0, new() { BaseItem = artifact });
        Referenceables.Add(artifact);
    }
}
