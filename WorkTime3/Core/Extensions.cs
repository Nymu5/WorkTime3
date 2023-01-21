using System.Collections;
using System.Collections.ObjectModel;
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

    public static TimeSpan SumTimeSpan<TValue>(this IReadOnlyCollection<TValue> collection,
        Func<TValue, TimeSpan> predicate)
    {
        TimeSpan sum = TimeSpan.Zero;
        foreach (TValue value in collection)
        {
            sum += predicate(value);
        }

        return sum;
    }

    public static double SumDouble<TValue>(this IReadOnlyCollection<TValue> collection,
        Func<TValue, double> predicate)
    {
        double sum = 0;
        foreach (TValue value in collection)
        {
            sum += predicate(value);
        }
        return sum;
    }

    public static string ToHourString(this TimeSpan time)
    {
        return $"{(int)time.TotalHours}:{time.Minutes.ToString("00")} h";
    }
}