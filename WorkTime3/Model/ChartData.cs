using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using ReactiveUI;

namespace MyTime.Model;

public class ChartData : ReactiveObject
{
    private int _year;
    public int Year
    {
        get => _year;
        set => this.RaiseAndSetIfChanged(ref _year, value);
    }

    
    private ISeries[] _series;
    public ISeries[] Series
    {
        get => _series;
        set => this.RaiseAndSetIfChanged(ref _series, value);
    }
    
    private Axis[] _xAxes;
    public Axis[] XAxes
    {
        get => _xAxes;
        set => this.RaiseAndSetIfChanged(ref _xAxes, value);
    }
    
    private Axis[] _yAxes;
    public Axis[] YAxes
    {
        get => _yAxes;
        set => this.RaiseAndSetIfChanged(ref _yAxes, value);
    }
}