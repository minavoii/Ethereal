using System;
using System.Collections.Generic;
using System.Linq;

namespace Ethereal.Utils;

public static class DictionaryExtensions
{
    /// <summary>
    /// Gets the key associated with the specified predicate.
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="predicate"></param>
    /// <param name="key"></param>
    /// <returns>true if the Dictionary<TKey, TValue> contains an element with the specified predicate; otherwise, false.</returns>
    public static bool TryGetKey<Key, Value>(
        this Dictionary<Key, Value> dictionary,
        Func<KeyValuePair<Key, Value>, bool> predicate,
        out Key key
    )
    {
        KeyValuePair<Key, Value> pair = dictionary.FirstOrDefault(predicate);
        key = pair.Key;

        return pair.Value != null;
    }
}
