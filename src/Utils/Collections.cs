using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethereal.Utils;

public static class DictionaryExtensions
{
    public static bool TryGetKey<Key, Value>(
        this Dictionary<Key, Value> dictionary,
        Func<KeyValuePair<Key, Value>, bool> predicate,
        out Key key
    )
    {
        var pair = dictionary.FirstOrDefault(predicate);
        key = pair.Key;

        return pair.Value != null;
    }
}
