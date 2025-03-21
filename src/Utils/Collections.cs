using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethereal.PluginUtils;

internal class Collections
{
    internal static bool TryTakeKey<Key, Value>(
        Dictionary<Key, Value> di,
        Func<KeyValuePair<Key, Value>, bool> predicate,
        out Key key
    )
    {
        var pair = di.FirstOrDefault(predicate);
        key = pair.Key;

        return pair.Value != null;
    }
}
