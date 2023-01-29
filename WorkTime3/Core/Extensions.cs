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

    public static TReturn[] AllChangeValues<TValue, TReturn, TComp>(this IReadOnlyCollection<TValue> array, Func<TValue, TReturn> predicate, Func<TReturn, TComp> comparer)
    {
        List<TReturn> list = new List<TReturn>();
        var sList = array.ToList();
        foreach (var t in sList)
        {
            if (!list.Any(c => comparer(c).Equals(comparer(predicate(t))))) list.Add(predicate(t));
        }
        list.Sort();
        return list.ToArray(); 
    }

    public static List<ChartData> ToChartDataList(this IReadOnlyCollection<Time> collection)
    {
        var list = new List<ChartData>();
        var employers = collection.AllChangeValues(x => x.Employer, x => x.Id);
        var years = collection.AllChangeValues(x => x.Start.Year, x => x);
        var sList = collection.ToList();
        if (!(employers.Length > 0 && years.Length > 0)) return list;
        var earningsCube = Graph.CreateEarningsCube(sList, employers);
        
        foreach (int year in years.Reverse())
        {
            list.Add(new ChartData
            {
                Series = Graph.CreateISeries(year, sList, employers, earningsCube),
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