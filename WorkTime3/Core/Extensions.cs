using System.Collections;
using System.Diagnostics.Metrics;
using DynamicData;
using MyTime.Model;

namespace MyTime.Core;

public static class Extensions
{
    public static void RemoveWhere<TValue, TKey>(this SourceCache<TValue, TKey> cache, Func<TValue, bool> predicate)
    {
        List<KeyValuePair<TKey, TValue>> forRemoval = cache.KeyValues.ToList();
        foreach (KeyValuePair<TKey,TValue> pair in forRemoval)
        {
            if (predicate(pair.Value))
            {
                cache.RemoveKey(pair.Key);
            }
        }
    }
}