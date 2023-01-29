using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;

namespace MyTime.Controller;

public class MainController : ReactiveObject
{
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

    public MainController()
    {
        Constants.Times
            .Connect()
            .Sort(SortExpressionComparer<Time>.Descending(t => t.Start))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var timesC)
            .Subscribe();

        Constants.Employers
            .Connect()
            .Sort(SortExpressionComparer<Employer>.Ascending(e => e.Name))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out var employersC)
            .Subscribe();

        var statsT = timesC
            .ToObservableChangeSet()
            .ToCollection();
        
        var statsE = employersC
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

        this.WhenAnyValue(x => x.ChartSelectedIndex)
            .Subscribe(x => ChartSelectedIndex = ChartData != null ? x < ChartData.Count ? x : 0 : 0); 
    }

    // Commands

    // Properties
    
    private int _chartSelectedIndex;
    public int ChartSelectedIndex
    {
        get => _chartSelectedIndex;
        private set => this.RaiseAndSetIfChanged(ref _chartSelectedIndex, value);
    }

    // Functions

}