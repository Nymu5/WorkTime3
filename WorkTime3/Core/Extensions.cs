using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using CommunityToolkit.Maui.Core.Extensions;
using DynamicData;
using DynamicData.Binding;
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

    public static TType[] AllValues<TValue, TType>(this IReadOnlyCollection<TValue> collection,
        Func<TValue, TType> predicate, bool reversed = false)
    {
        List<TType> list = new List<TType>();
        foreach (TValue value in collection)
        {
            if (!list.Contains(predicate(value))) list.Add(predicate(value));
        }
        list.Sort();
        if (reversed) list.Reverse();
        return list.ToArray();
    }

    public static string ToHourString(this TimeSpan time)
    {
        return $"{(int)time.TotalHours}:{time.Minutes.ToString("00")} h";
    }
}