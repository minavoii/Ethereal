using System.Collections.Concurrent;
using UnityEngine;

namespace Ethereal.API;

public static class MonsterTypes
{
    private static readonly ConcurrentQueue<(EMonsterType monsterType, Sprite typeIcon)> Queue =
        new();

    private static bool IsReady = false;

    internal static void ReadQueue()
    {
        IsReady = true;

        while (Queue.TryDequeue(out var item))
            UpdateIcon(item.monsterType, item.typeIcon);
    }

    public static MonsterType Get(EMonsterType monsterType)
    {
        return GameController.Instance.MonsterTypes.Find(x => x?.Type == monsterType);
    }

    public static bool TryGet(EMonsterType monsterType, out MonsterType result)
    {
        if (!IsReady)
            result = null;
        else
            result = Get(monsterType);

        return result != null;
    }

    public static void UpdateIcon(EMonsterType monsterType, Sprite typeIcon)
    {
        // Defer loading until ready
        if (GameController.Instance?.CompleteMonsterList == null || !IsReady)
        {
            Queue.Enqueue((monsterType, typeIcon));
            return;
        }

        if (TryGet(monsterType, out var type))
            type.TypeIcon = typeIcon;
    }
}
