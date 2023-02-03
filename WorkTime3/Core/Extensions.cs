#nullable enable
using MyTime.Model;

namespace MyTime.Core;

public static class Extensions
{
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

    private static TReturn[] AllChangeValues<TValue, TReturn, TComp>(this IReadOnlyCollection<TValue> array, Func<TValue, TReturn> predicate, Func<TReturn, TComp> comparer)
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
                Year = year,
                HtmlElement = Graph.BuildEarningsHtml(year, sList, employers, earningsCube),
            });
        }
        
        return list;
    }

    public static string ToHourString(this TimeSpan time)
    {
        return $"{(int)time.TotalHours}:{time.Minutes:00} h";
    }
    
    public static Microsoft.Maui.Controls.View? GetFocusedView(this Microsoft.Maui.Controls.View control)
    {
        Microsoft.Maui.Controls.View? view = null;
        if (control.IsFocused) return control;
        var result = control.GetScreenCoords();
        var type = Convert.ChangeType(control, control.GetType());

        Console.WriteLine(control.ToString());
        return view;
    }
    
    /// <summary>
    /// A view's default X- and Y-coordinates are LOCAL with respect to the boundaries of its parent,
    /// and NOT with respect to the screen. This method calculates the SCREEN coordinates of a view.
    /// The coordinates returned refer to the top left corner of the view.
    /// </summary>
    public static Point GetScreenCoords(this VisualElement view)
    {
        var result = new Point(view.X, view.Y);
        while (view.Parent is VisualElement parent)
        {
            result = result.Offset(parent.X, parent.Y);
            view = parent;
        }
        return result;
    }
}