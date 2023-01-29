using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using MyTime.Core;
using MyTime.Model;
using MyTime.View;
using ReactiveUI;

namespace MyTime.Controller;

public class TimeController : ReactiveObject
{
    public TimeController()
    {
        CreateTimeCommand = new Command(execute: CreateTimeTask);
        SelectionChangedCommand = new Command(execute: SelectionChangedTask);
        DateTime now = DateTime.Today;
        FilterStart = new DateTime(now.Year, 1, 1);
        FilterEnd = new DateTime(now.Year + 1, 1, 1) - TimeSpan.FromMicroseconds(1);

        RefreshCommand = new Command(execute: RefreshTask);

        Func<Time, bool> SearchTermFilter(string text) => time => (String.IsNullOrWhiteSpace(text) ||
                                                                   time.Text.ToLower().Contains(text.ToLower()) ||
                                                                   time.Employer.Name.ToLower()
                                                                       .Contains(text.ToLower()));

        var filterPredicate = this.WhenAnyValue(x => x.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .Select(SearchTermFilter);

        Func<Time, bool> StartTimeFilter(DateTime date) => time => time.Start >= FilterStart;
        var filterStartPredicate = this.WhenAnyValue(x => x.FilterStart)
            .Throttle(TimeSpan.FromMicroseconds(250), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .Select(StartTimeFilter);

        Func<Time, bool> EndTimeFilter(DateTime date) => time => time.Start <= FilterEnd;
        var filterEndPredicate = this.WhenAnyValue(x => x.FilterEnd)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .Select(EndTimeFilter);

        Constants.Times
            .Connect()
            .Filter(filterPredicate)
            .Filter(filterStartPredicate)
            .Filter(filterEndPredicate)
            .Sort(SortExpressionComparer<Time>.Descending(t => t.Start))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out Times)
            .Subscribe();

        var stats = Times
            .ToObservableChangeSet()
            .ToCollection();
        
        _filterWorkedAmount = stats
            .Select(x => x.Count)
            .ToProperty(this, x => x.FilterWorkedAmount);
        _filterWorkedTotal = stats
            .Select(x => x.SumDouble(time => time.Earned).ToString("C"))
            .ToProperty(this, x => x.FilterWorkedTotal);
        _filterWorkedTime = stats
            .Select(x => x.SumTimeSpan(time => time.Duration).ToHourString())
            .ToProperty(this, x => x.FilterWorkedTime);
        
    }

    private async void RefreshTask()
    {
        Constants.Times.Clear();
        Constants.Times.AddOrUpdate(await Constants.Database.GetTimesAsync());
        IsRefreshing = false;
    }

    private async void SelectionChangedTask()
    {
        Console.WriteLine("Selection Changed");
        if (SelectedTime != null)
        {
            await Shell.Current.GoToAsync("AddTimePage", new Dictionary<string, object> { { "Time", SelectedTime }, });
            SelectedTime = null;
        }
    }

    private async void CreateTimeTask()
    {
        await Shell.Current.GoToAsync(nameof(AddTimePage));
    }

    // Commands
    public ICommand CreateTimeCommand { get; }
    public ICommand SelectionChangedCommand { get; }
    public ICommand RefreshCommand { get; }

    // Properties
    private Time _selectedTime;
    public Time SelectedTime
    {
        get => _selectedTime;
        set => this.RaiseAndSetIfChanged(ref _selectedTime, value);
    }
    
    public readonly ReadOnlyObservableCollection<Time> Times;

    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => this.RaiseAndSetIfChanged(ref _isRefreshing, value);
    }

    private string _searchTerm = String.Empty;
    public string SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    private readonly ObservableAsPropertyHelper<int> _filterWorkedAmount;
    public int FilterWorkedAmount => _filterWorkedAmount.Value;

    private readonly ObservableAsPropertyHelper<string> _filterWorkedTotal;
    public string FilterWorkedTotal => _filterWorkedTotal.Value;
    private readonly ObservableAsPropertyHelper<string> _filterWorkedTime;
    public string FilterWorkedTime => _filterWorkedTime.Value;


    private readonly DateTime _filterStart;
    public DateTime FilterStart
    {
        get => _filterStart;
        private init => this.RaiseAndSetIfChanged(ref _filterStart, value);
    }
    
    private readonly DateTime _filterEnd;
    public DateTime FilterEnd
    {
        get => _filterEnd;
        private init => this.RaiseAndSetIfChanged(ref _filterEnd, value);
    }

    // Functions
}