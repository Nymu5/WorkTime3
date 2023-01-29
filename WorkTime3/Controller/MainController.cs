using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using CommunityToolkit.Maui.Core.Extensions;
using DynamicData;
using DynamicData.Binding;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;
using SkiaSharp;
using Extensions = MyTime.Core.Extensions;

namespace MyTime.Controller;

public class MainController : ReactiveObject
{
    public readonly ReadOnlyObservableCollection<Time> TimesC;
    public readonly ReadOnlyObservableCollection<Employer> EmployersC;
    public readonly ReadOnlyObservableCollection<ChartData> ChartC;

    private readonly ObservableAsPropertyHelper<int> _statsEmployers;
    public int StatsEmployers => _statsEmployers.Value;
    
    private readonly ObservableAsPropertyHelper<string> _statsHours;
    public string StatsHours => _statsHours.Value;
    
    private readonly ObservableAsPropertyHelper<string> _statsTotal;
    public string StatsTotal => _statsTotal.Value;
    
    private readonly ObservableAsPropertyHelper<int> _statsTimes;
    public int StatsTimes => _statsTimes.Value;
    
    private readonly ObservableAsPropertyHelper<List<ChartData>> _chartData;
    public List<ChartData> ChartData => _chartData.Value;
    
    //private readonly ObservableAsPropertyHelper<ObservableCollection<int>> _years;
    //public ObservableCollection<int> Years => _years.Value;
    
    private int[] _years;
    public int[] Years
    {
        get => _years;
        set => this.RaiseAndSetIfChanged(ref _years, value);
    }
    
    private bool _refresher = false;
    public bool Refresher
    {
        get => _refresher;
        set => this.RaiseAndSetIfChanged(ref _refresher, value);
    }
    
    private int _yearSelected;
    public int YearSelected
    {
        get => _yearSelected;
        set => this.RaiseAndSetIfChanged(ref _yearSelected, value);
    }

    public MainController()
    {
        Years = Array.Empty<int>();
        Constants.Times
            .Connect()
            .Sort(SortExpressionComparer<Time>.Descending(t => t.Start))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out TimesC)
            .Subscribe(x =>
            {
                Constants.Charts.Clear();
                Constants.Charts.AddOrUpdate(x.ChartMapper());
            });

        Constants.Charts
            .Connect()
            .Sort(SortExpressionComparer<ChartData>.Descending(c => c.Year))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out ChartC)
            .Subscribe(x => Console.WriteLine(x.Count));

        Constants.Employers
            .Connect()
            .Sort(SortExpressionComparer<Employer>.Ascending(e => e.Name))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out EmployersC)
            .Subscribe();

        //Constants.Times
        //    .Connect()
        //    .Select(Graph.ChartMapper)
        //    .Sort(SortExpressionComparer<ChartData>.Descending(c => c.Year))
        //    .ObserveOn(RxApp.MainThreadScheduler)
        //    .Bind(out ChartC)
        //    .Subscribe();


        var statsT = TimesC
            .ToObservableChangeSet()
            .ToCollection();
        
        var statsE = EmployersC
            .ToObservableChangeSet()
            .ToCollection();

        _chartData = statsT
            .Select(x => x.ToChartDataList())
            .ToProperty(this, x => x.ChartData);

        _statsEmployers = statsE
            .Select(x => x.Count)
            .ToProperty(this, x => x.StatsEmployers);

        _statsHours = statsT
            .Select(x => x.SumTimeSpan(t => t.Duration).ToHourString())
            .ToProperty(this, x => x.StatsHours);

        _statsTotal = statsT
            .Select(x => x.SumDouble(t => t.Earned).ToString("C"))
            .ToProperty(this, x => x.StatsTotal);

        _statsTimes = statsT
            .Select(x => x.Count)
            .ToProperty(this, x => x.StatsTimes);

        TimesC
            .ToObservableChangeSet()
            .ToCollection()
            .Select(x => x.AllValues(t => t.Start.Year, true))
            .Subscribe(x => Years = x);

        this.WhenAnyValue(x => x.Years)
            .Subscribe(x =>
            {
                if (x.Length <= 0) return;
                YearSelected = x.Contains(DateTime.Now.Year) ? DateTime.Now.Year : x.First();
            });
    }

    // Commands

    // Properties

    // Functions

}