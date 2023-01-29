using DynamicData;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyTime.Model;

namespace MyTime.Core;

public static class Graph
{
    public static ISeries[] CreateISeries(int year, List<Time> times,
        Employer[] employers, double[,,] earningsCube)
    {
        ISeries[] iSeries = new ISeries[employers.Length];
    
        if (!(employers.Length > 0 && times.Count > 0)) return iSeries;
            
        if (times.Max(t => t.Start.Year) < year || times.Min(t => t.Start.Year) > year)
            year = times.Max(t => t.Start.Year);
        Console.WriteLine("new graph");

        for (int i = 0; i < employers.Length; i++)
        {
            double[] values = new double[12];
            for (int j = 0; j < 12; j++)
            {
                values[j] = earningsCube[i, year - times.Min(t => t.Start.Year), j];
                iSeries[i] = new StackedColumnSeries<double>
                {
                    Values = values,
                    Name = employers[i].Name,
                    TooltipLabelFormatter = (chartPoint) =>
                        $"{chartPoint.Context.Series.Name}: {chartPoint.PrimaryValue:C}",
                    Stroke = null,
                    Fill = new SolidColorPaint(Constants.Colors[i % 10])
                };
            }
        }

        return iSeries;
    }

    public static double[,,] CreateEarningsCube(List<Time> times,
        Employer[] employers)
    {
        
        int minYear = times.Min(t => t.Start.Year);
        int maxYear = times.Max(t => t.Start.Year);
        int years = maxYear - minYear + 2;

        double[,,] earningsCube = new double[employers.Length + 1, years, 13];

        foreach (Time time in times)
        {
            int pos1 = employers.IndexOf(employers.FirstOrDefault(e => e.Id == time.Employer.Id));
            int pos2 = time.Start.Year - minYear;
            int pos3 = time.Start.Month - 1;
            
            earningsCube[pos1, pos2, pos3] += time.Earned;
            earningsCube[employers.Length, pos2, pos3] += time.Earned;
            earningsCube[pos1, years - 1, pos3] += time.Earned;
            earningsCube[pos1, pos2, 12] += time.Earned;
            earningsCube[employers.Length, years - 1, pos3] += time.Earned;
            earningsCube[employers.Length, pos2, 12] += time.Earned;
            earningsCube[pos1, years - 1, 12] += time.Earned;
            earningsCube[employers.Length, years - 1, 12] += time.Earned;
        }

        return earningsCube;
    }
}