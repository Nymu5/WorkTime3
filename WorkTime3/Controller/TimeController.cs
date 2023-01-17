using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;

namespace MyTime.Controller;

public class TimeController : ControllerBase
{
    private MyTimeDatabase _db;

    public TimeController()
    {
        TimesCache = new SourceCache<Time, string>(t => t.Id);
        _db = new MyTimeDatabase();
        CreateTimeCommand = new Command(execute: async () => { await Shell.Current.GoToAsync("AddTimePage"); });
        RefreshCommand = new Command(execute: async () =>
        {
            TimesCache.AddOrUpdate(await _db.GetTimesAsync());
            var disposable = TimesCache
                .Connect()
                .Sort(SortExpressionComparer<Time>.Descending(t => t.Start))
                .Filter(t =>
                    (t.Text.ToLower().Contains(SearchValue.ToLower()) ||
                    t.Employer.Name.ToLower().Contains(SearchValue.ToLower())) || String.IsNullOrWhiteSpace(SearchValue) ||
                    t.TimeStartString.ToLower().Contains(SearchValue.ToLower()) || t.TimeEndString.ToLower().Contains(SearchValue.ToLower())) 
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _times)
                .DisposeMany()
                .Subscribe();
            OnPropertyChanged(nameof(Times));
            IsRefreshing = false;
        });
        SelectionChangedCommand = new Command(execute: async () =>
        {
            if (SelectedTime != null)
            {
                await Shell.Current.GoToAsync("AddTimePage", new Dictionary<string, object>
                {
                    { "Time", SelectedTime },
                    { "TimeId", SelectedTime.Id }
                });
                SelectedTime = null;
            }
        });
        ClearSearch = new Command(execute: () =>
        {
            SearchValue = String.Empty;
        });
    }

    // Commands
    public ICommand CreateTimeCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SelectionChangedCommand { get; }
    public ICommand ClearSearch { get; }

    // Properties
    private Time _selectedTime;

    public Time SelectedTime
    {
        get => _selectedTime;
        set => SetProperty(ref _selectedTime, value);
    }
    
    private SourceCache<Time, string> _timesCache;
    public SourceCache<Time, string> TimesCache
    {
        get => _timesCache;
        set => SetProperty(ref _timesCache, value);
    }

    private ReadOnlyObservableCollection<Time> _times;
    public ReadOnlyObservableCollection<Time> Times
    {
        get => _times;
        set => SetProperty(ref _times, value);
    }

    private bool _isRefreshing;

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }
    
    private string _searchValue = "";
    public string SearchValue
    {
        get => _searchValue;
        set
        {
            SetProperty(ref _searchValue, value);
            OnPropertyChanged(nameof(Times));
        }
    }

}