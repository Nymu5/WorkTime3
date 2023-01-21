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

namespace MyTime.Controller;

public class MainController : ReactiveObject
{
    public readonly ReadOnlyObservableCollection<Time> TimesC;
    public readonly ReadOnlyObservableCollection<Employer> EmployersC;

    private readonly ObservableAsPropertyHelper<int> _statsEmployers;
    public int StatsEmployers => _statsEmployers.Value;
    
    private readonly ObservableAsPropertyHelper<string> _statsHours;
    public string StatsHours => _statsHours.Value;
    
    private readonly ObservableAsPropertyHelper<string> _statsTotal;
    public string StatsTotal => _statsTotal.Value;
    
    private readonly ObservableAsPropertyHelper<int> _statsTimes;
    public int StatsTimes => _statsTimes.Value;
    
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
        RefreshStats = new Command(execute: () => Refresher = !Refresher );
        Years = Array.Empty<int>();
        Constants.Times
            .Connect()
            .Sort(SortExpressionComparer<Time>.Descending(t => t.Start))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out TimesC)
            .Subscribe();

        Constants.Employers
            .Connect()
            .Sort(SortExpressionComparer<Employer>.Ascending(e => e.Name))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out EmployersC)
            .Subscribe();

        var statsT = TimesC
            .ToObservableChangeSet()
            .ToCollection();
        
        var statsE = EmployersC
            .ToObservableChangeSet()
            .ToCollection();

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

        //_series = TimesC
        //    .ToObservableChangeSet()
        //    .ToCollection()
        //    .Select(x => CreateGraph())
        //    .ToProperty(this, x => x.Series);

        this.WhenAnyValue(x => x.YearSelected, 
                x => x.EmployersC, 
                x => x.TimesC, 
                x => x.Refresher,
                (year, employers, times, refresher) => 
                    Graph.CreateISeries(year, times, employers))
            .Subscribe(x => Series = x);
        
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
    
    

    public Axis[] XAxes { get; set; } =
    {
        new Axis
        {
            Labels = new string[]
                { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
            LabelsRotation = 90,
            SeparatorsPaint = new SolidColorPaint(new SKColor(200, 200, 200)),
            SeparatorsAtCenter = false,
            TicksPaint = new SolidColorPaint(new SKColor(35, 35, 35)),
            TicksAtCenter = true,
            TextSize = 40,
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis
        {
            TextSize = 40,
            Labeler = Labelers.Currency,
            MinLimit = 0,
        }
    };

    // Commands
    
    public ICommand RefreshStats { get; }

    // Properties
    
    private ISeries[] _series;
    public ISeries[] Series
    {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
    }

    private double[,,] _earningsCube;
    public double[,,] EarningsCube
    {
        get => _earningsCube;
        set => this.RaiseAndSetIfChanged(ref _earningsCube, value);
    }

    private TimeSpan[,,] _timesCube;

    public TimeSpan[,,] TimesCube
    {
        get => _timesCube;
        set => this.RaiseAndSetIfChanged(ref _timesCube, value);
    }



    // Functions

}