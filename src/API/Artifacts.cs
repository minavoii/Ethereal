using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ethereal.Attributes;
using Ethereal.Classes.Builders;

namespace Ethereal.API;

[BasicAPI]
public static partial class Artifacts
{
    /// <summary>
    /// Get an artifact by id.
    /// </summary>
    /// <param name="id"></param>
    public static async Task<Consumable?> Get(int id) => await Get(x => x?.BaseItem.ID == id);

    /// <summary>
    /// Get an artifact by id.
    /// </summary>
    /// <param name="name"></param>
    public static async Task<Consumable?> Get(string name) =>
        await Get(x => x?.BaseItem.Name == name);

    /// <summary>
    /// Get an artifact using a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static async Task<Consumable?> Get(Predicate<ItemManager.BaseItemInstance?> predicate)
    {
        await WhenReady();

        return GameController.Instance.ItemManager.Consumables.Find(predicate)?.BaseItem
            as Consumable;
    }

    public static async Task<List<Consumable>> GetAll()
    {
        await WhenReady();

        return
        [
            .. GameController.Instance.ItemManager.Consumables.Select(x =>
                (x.BaseItem as Consumable)!
            ),
        ];
    }

    /// <summary>
    /// Create a new artifact and add it to the game's data
    /// </summary>
    /// <param name="artifact"></param>
    public static async Task<Consumable> Add(ArtifactBuilder artifact) =>
        await Add(artifact.Build());

    /// <summary>
    /// Create a new artifact and add it to the game's data
    /// </summary>
    /// <param name="artifact"></param>
    public static async Task<Consumable> Add(Consumable artifact)
    {
        await WhenReady();

        GameController.Instance.ItemManager.Consumables.Insert(0, new() { BaseItem = artifact });
        await Referenceables.Add(artifact);

        return artifact;
    }
}
