using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;
using SkiaSharp;

namespace MyTime.Controller;

public class MainController : ReactiveObject
{
    private MyTimeDatabase _db = new MyTimeDatabase();
    public readonly ReadOnlyObservableCollection<Time> TimesC;
    public readonly ReadOnlyObservableCollection<Employer> EmployersC; 
    int _minYear;
    int _maxYear;
    int _years;
    int _employerCount;
    
    private readonly ObservableAsPropertyHelper<int> _statsEmployers;
    public int StatsEmployers => _statsEmployers.Value;
    
    private readonly ObservableAsPropertyHelper<string> _statsHours;
    public string StatsHours => _statsHours.Value;
    
    private readonly ObservableAsPropertyHelper<string> _statsTotal;
    public string StatsTotal => _statsTotal.Value;
    
    private readonly ObservableAsPropertyHelper<int> _statsTimes;
    public int StatsTimes => _statsTimes.Value;
    
    

    public MainController()
    {
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

        _series = TimesC
            .ToObservableChangeSet()
            .ToCollection()
            .Select(x => CreateGraph())
            .ToProperty(this, x => x.Series);

        //_series = this.WhenAnyValue(x => x.TimesC, x => x.EmployersC)
        //    .ToObservableChangeSet()
        //    .ToCollection()
        //    .Select((data) => CreateGraph(data))
        //    .ToProperty(this, x => x.Series);
        //var employersO = EmployersC
        //    .ToObservableChangeSet()
        //    .ToCollection();
//
        //var timesO = TimesC
        //    .ToObservableChangeSet()
        //    .ToCollection();
        //
        //_series = this.WhenAnyValue(x => x.TimesC, x => x.EmployersC, (t, e) => new object[] { t, e })
        //    .Select(x => CreateGraph(x))
        //    .DistinctUntilChanged()
        //    .ToProperty(this, x => x.Series);

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

    // Properties

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
    
    private SKColor[] _colors = new SKColor[]
    {
        SKColor.Parse("F8B195"),
        SKColor.Parse("F67280"),
        SKColor.Parse("6C5B7B"),
        SKColor.Parse("C06C84"),
        SKColor.Parse("355C7D"),
        SKColor.Parse("FE4365"),
        SKColor.Parse("FC9D9A"),
        SKColor.Parse("F9CDAD"),
        SKColor.Parse("C8C8A9"),
        SKColor.Parse("83AF9B"),
    };
    
    private readonly ObservableAsPropertyHelper<ISeries[]> _series;
    public ISeries[] Series => _series.Value;

    // Functions

    private ISeries[] CreateGraph()
    {
        Console.WriteLine("awdunawiodubawodiubawodiuyvbaow8iduybao8wudbyawdb!!!");

        ReadOnlyObservableCollection<Employer> employer = EmployersC;
        ReadOnlyObservableCollection<Time> times = TimesC;
        
        Console.WriteLine(employer.Count);
        Console.WriteLine(times.Count);
        ISeries[] series = new ISeries[employer.Count];
        if (employer.Count > 0 && times.Count > 0)
        {
            _minYear = times.Min(t => t.Start.Year);
            _maxYear = times.Max(t => t.Start.Year);
            _years = _maxYear - _minYear + 2;
            _employerCount = employer.Count + 1;

            List<int> tempYears = new List<int>();
            for (int i = _maxYear; i >= _minYear; i--)
            {
                tempYears.Add(i);
            }

            EarningsCube = new Double[_employerCount, _years, 13];
            TimesCube = new TimeSpan[_employerCount, _years, 13];

            foreach (var (time, i) in times.WithIndex())
            {
                int pos1 = employer.IndexOf(employer.FirstOrDefault(e => e.Id == time.Employer.Id));
                int pos2 = time.Start.Year - _minYear;
                int pos3 = time.Start.Month - 1;

                EarningsCube[pos1, pos2, pos3] += time.Earned;
                EarningsCube[employer.Count, pos2, pos3] += time.Earned;
                EarningsCube[pos1, _years - 1, pos3] += time.Earned;
                EarningsCube[pos1, pos2, 12] += time.Earned;
                EarningsCube[employer.Count, _years - 1, pos3] += time.Earned;
                EarningsCube[employer.Count, pos2, 12] += time.Earned;
                EarningsCube[pos1, _years - 1, 12] += time.Earned;
                EarningsCube[employer.Count, _years - 1, 12] += time.Earned;

                TimesCube[pos1, pos2, pos3] += time.Duration;
                TimesCube[employer.Count, pos2, pos3] += time.Duration;
                TimesCube[pos1, _years - 1, pos3] += time.Duration;
                TimesCube[pos1, pos2, 12] += time.Duration;
                TimesCube[employer.Count, _years - 1, pos3] += time.Duration;
                TimesCube[employer.Count, pos2, 12] += time.Duration;
                TimesCube[pos1, _years - 1, 12] += time.Duration;
                TimesCube[employer.Count, _years - 1, 12] += time.Duration;
            }
            
            for (int i = 0; i < employer.Count; i++)
            {
                double[] vals = new double[12];
                for (int j = 0; j < 12; j++)
                {
                    vals[j] = EarningsCube[i, _years - 2, j];
                    series[i] = new StackedColumnSeries<double>
                    {
                        Values = vals,
                        Name = employer[i].Name,
                        TooltipLabelFormatter = (chartPoint) =>
                            $"{chartPoint.Context.Series.Name}: {(chartPoint.PrimaryValue.ToString("C"))}",
                        Stroke = null,
                        Fill = new SolidColorPaint(_colors[i % 10]),
                    };
                }
            }
        }

        return series;
    }
}