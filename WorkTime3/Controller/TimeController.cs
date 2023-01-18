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

[QueryProperty(nameof(DeleteTime), "DeleteTime")]
public class TimeController : ReactiveObject
{
    private MyTimeDatabase _db;

    public TimeController()
    {
        _myTimes = new SourceCache<Time, string>(t => t.Id);
        _db = new MyTimeDatabase();
        CreateTimeCommand = new Command(execute: async () => { await Shell.Current.GoToAsync("AddTimePage"); });
        RefreshCommand = new Command(execute: async () =>
        {
            Console.WriteLine(_myTimes.Count);
            Console.WriteLine("Updating list!");
            _myTimes.AddOrUpdate(await _db.GetTimesAsync());
            IsRefreshing = false;
        });
        SelectionChangedCommand = new Command(execute: async () =>
        {
            if (SelectedTime != null)
            {
                await Shell.Current.GoToAsync("AddTimePage", new Dictionary<string, object>
                {
                    { "Time", SelectedTime },
                    { "TimeId", SelectedTime.Id },
                    { "MyTimes", _myTimes }
                });
                SelectedTime = null;
            }
        });
        ClearSearch = new Command(execute: () =>
        {
            SearchTerm = String.Empty;
        });

        Func<Time, bool> SearchTermFilter(string text) => time => String.IsNullOrWhiteSpace(text) ||
                                                                  time.Text.ToLower().Contains(text.ToLower()) ||
                                                                  time.Employer.Name.ToLower().Contains(text.ToLower());

        var filterPredicate = this.WhenAnyValue(x => x.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .Select(SearchTermFilter);

        var disposable = _myTimes
            .Connect()
            .Filter(filterPredicate)
            .Sort(SortExpressionComparer<Time>.Descending(t => t.Start))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out Times)
            .Subscribe();
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
        set => this.RaiseAndSetIfChanged(ref _selectedTime, value);
    }
    
    private SourceCache<Time, string> _myTimes;
    public ReadOnlyObservableCollection<Time> Times;

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

    public Time DeleteTime
    {
        set => _myTimes.Remove(value);
    }

}