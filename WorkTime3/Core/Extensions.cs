using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Reactive.Linq;
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

    public static TReturn[] AllChangeValues<TValue, TType, TReturn, TComp>(this Change<TValue, TType>[] array, Func<TValue, TReturn> predicate, Func<TReturn, TComp> comparer)
    {
        List<TReturn> list = new List<TReturn>();
        for (int i = 0; i < array.Length; i++)
        {
            if (!list.Any(c => comparer(c).Equals(comparer(predicate(array[i].Current))))) list.Add(predicate(array[i].Current));
        }
        list.Sort();
        return list.ToArray(); 
    }

    public static List<ChartData> ToChartDataList(this IReadOnlyCollection<Time> collection)
    {
        var list = new List<ChartData>();
        return list;
    }

    public static List<ChartData> ChartMapper(this IChangeSet<Time, string> times)
    {
        var list = new List<ChartData>();
        var array = times.ToArray();

        var employers = times.ToArray().AllChangeValues(t => t.Employer, e => e.Id);
        var years = times.ToArray().AllChangeValues(t => t.Start.Year, y => y);

        if (!(employers.Length > 0 && years.Length > 0)) return list;

        var earningsCube = Graph.CreateEarningsCube(times, employers);

        foreach (int year in years)
        {
            list.Add(new ChartData
            {
                Series = Graph.CreateISeries(year, times, employers, earningsCube),
                Year = year,
                XAxes = Constants.XAxes,
                YAxes = Constants.YAxes
            });
        }

        return list;
    }

    public static string ToHourString(this TimeSpan time)
    {
        return $"{(int)time.TotalHours}:{time.Minutes.ToString("00")} h";
    }
}