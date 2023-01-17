using System.Windows.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyTime.Core;
using MyTime.Model;
using SkiaSharp;

namespace MyTime.Controller;

public class MainController : ControllerBase
{
    private MyTimeDatabase _db = new MyTimeDatabase();
    int _minYear;
    int _maxYear;
    int _years;
    int _employerCount;

    public MainController()
    {
        SKColor[] colors = new SKColor[]
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
        LoadDashboardCommand = new Command(execute: async () =>
        {
            Employers = await _db.GetEmployersAsync();
            Times = await _db.GetTimesAsync();

            if (Employers.Count > 0 && Times.Count > 0)
            {
                _minYear = Times.Min(t => t.Start.Year);
                _maxYear = Times.Max(t => t.Start.Year);
                _years = _maxYear - _minYear + 2;
                _employerCount = Employers.Count + 1;


                EarningsCube = new Double[_employerCount, _years, 13];
                TimesCube = new TimeSpan[_employerCount, _years, 13];

                foreach (var (time, i) in Times.WithIndex())
                {
                    int pos1 = Employers.IndexOf(Employers.FirstOrDefault(e => e.Id == time.Employer.Id));
                    int pos2 = time.Start.Year - _minYear;
                    int pos3 = time.Start.Month - 1;

                    EarningsCube[pos1, pos2, pos3] += time.Earned;
                    EarningsCube[Employers.Count, pos2, pos3] += time.Earned;
                    EarningsCube[pos1, _years - 1, pos3] += time.Earned;
                    EarningsCube[pos1, pos2, 12] += time.Earned;
                    EarningsCube[Employers.Count, _years - 1, pos3] += time.Earned;
                    EarningsCube[Employers.Count, pos2, 12] += time.Earned;
                    EarningsCube[pos1, _years - 1, 12] += time.Earned;
                    EarningsCube[Employers.Count, _years - 1, 12] += time.Earned;

                    TimesCube[pos1, pos2, pos3] += time.Duration;
                    TimesCube[Employers.Count, pos2, pos3] += time.Duration;
                    TimesCube[pos1, _years - 1, pos3] += time.Duration;
                    TimesCube[pos1, pos2, 12] += time.Duration;
                    TimesCube[Employers.Count, _years - 1, pos3] += time.Duration;
                    TimesCube[Employers.Count, pos2, 12] += time.Duration;
                    TimesCube[pos1, _years - 1, 12] += time.Duration;
                    TimesCube[Employers.Count, _years - 1, 12] += time.Duration;
                }

                OnPropertyChanged(nameof(Times));
                OnPropertyChanged(nameof(TotalEarnings));
                OnPropertyChanged(nameof(TotalHours));
                Series = new ISeries[Employers.Count];
                for (int i = 0; i < Employers.Count; i++)
                {
                    double[] vals = new double[12];
                    for (int j = 0; j < 12; j++)
                    {
                        vals[j] = EarningsCube[i, _years - 2, j];
                        Series[i] = new StackedColumnSeries<double>
                        {
                            Values = vals,
                            Name = Employers[i].Name,
                            TooltipLabelFormatter = (chartPoint) =>
                                $"{chartPoint.Context.Series.Name}: {(chartPoint.PrimaryValue.ToString("C"))}",
                            Stroke = null,
                            Fill = new SolidColorPaint(colors[i % 10]),
                        };
                    }
                }
            }
        });
    }

    private ISeries[] _series;

    public ISeries[] Series
    {
        get => _series;
        set => SetProperty(ref _series, value);
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
        }
    };

    // Commands
    public ICommand LoadDashboardCommand { get; }

    // Properties
    private List<Employer> _employers;

    public List<Employer> Employers
    {
        get => _employers;
        set { SetProperty(ref _employers, value); }
    }

    private List<Time> _times;

    public List<Time> Times
    {
        get => _times;
        set => SetProperty(ref _times, value);
    }

    private double[,,] _earningsCube;

    public double[,,] EarningsCube
    {
        get => _earningsCube;
        set => SetProperty(ref _earningsCube, value);
    }

    private TimeSpan[,,] _timesCube;

    public TimeSpan[,,] TimesCube
    {
        get => _timesCube;
        set => SetProperty(ref _timesCube, value);
    }

    public string TotalEarnings =>
        (EarningsCube != null ? EarningsCube[_employerCount - 1, _years - 1, 12] : 0).ToString("C");

    public string TotalHours =>
        Constants.TsFormatter(TimesCube != null ? TimesCube[_employerCount - 1, _years - 1, 12] : TimeSpan.Zero);

    // Functions
}