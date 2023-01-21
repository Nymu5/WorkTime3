using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyTime.Model;

namespace MyTime.Core;

public static class Graph
{
    public static ISeries[] CreateISeries(int year, ReadOnlyObservableCollection<Time> times,
        ReadOnlyObservableCollection<Employer> employers)
    {
        ISeries[] iSeries = new ISeries[employers.Count];
    
        if (!(employers.Count > 0 && times.Count > 0)) return iSeries;
            
        if (times.Max(t => t.Start.Year) < year || times.Min(t => t.Start.Year) > year)
            year = times.Max(t => t.Start.Year);
        Console.WriteLine("new graph");
        

        double[,,] earningsCube = CreateEarningsCube(times, employers);

        for (int i = 0; i < employers.Count; i++)
        {
            double[] vals = new double[12];
            for (int j = 0; j < 12; j++)
            {
                vals[j] = earningsCube[i, year - times.Min(t => t.Start.Year), j];
                iSeries[i] = new StackedColumnSeries<double>
                {
                    Values = vals,
                    Name = employers[i].Name,
                    TooltipLabelFormatter = (chartPoint) =>
                        $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue.ToString("C")}",
                    Stroke = null,
                    Fill = new SolidColorPaint(Constants.Colors[i % 10])
                };
            }
        }

        return iSeries;
    }

    public static double[,,] CreateEarningsCube(ReadOnlyObservableCollection<Time> times,
        ReadOnlyObservableCollection<Employer> employers)
    {
        int minYear = times.Min(t => t.Start.Year);
        int maxYear = times.Max(t => t.Start.Year);
        int years = maxYear - minYear + 2;
        int employersCount = employers.Count + 1;

        double[,,] earningsCube = new double[employers.Count + 1, years, 13];

        foreach (var (time, i) in times.WithIndex())
        {
            int pos1 = employers.IndexOf(employers.FirstOrDefault(e => e.Id == time.Employer.Id));
            int pos2 = time.Start.Year - minYear;
            int pos3 = time.Start.Month - 1;
            
            earningsCube[pos1, pos2, pos3] += time.Earned;
            earningsCube[employers.Count, pos2, pos3] += time.Earned;
            earningsCube[pos1, years - 1, pos3] += time.Earned;
            earningsCube[pos1, pos2, 12] += time.Earned;
            earningsCube[employers.Count, years - 1, pos3] += time.Earned;
            earningsCube[employers.Count, pos2, 12] += time.Earned;
            earningsCube[pos1, years - 1, 12] += time.Earned;
            earningsCube[employers.Count, years - 1, 12] += time.Earned;
        }

        return earningsCube;
    }
}