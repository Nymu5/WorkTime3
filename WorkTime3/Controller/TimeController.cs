using System.Collections.ObjectModel;
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
        _db = new MyTimeDatabase();
        CreateTimeCommand = new Command(execute: async () =>
        {
            await Shell.Current.GoToAsync("AddTimePage");
        });
        RefreshCommand = new Command(execute: async () =>
        {
            SourceCache<Time, string> times = await _db.GetTimesAsync();
            var disposable = times
                .Connect()
                .Sort(SortExpressionComparer<Time>.Descending(t => t.Start))
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
    }
    
    // Commands
    public ICommand CreateTimeCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SelectionChangedCommand { get; }
    
    // Properties
    private Time _selectedTime;
    public Time SelectedTime
    {
        get => _selectedTime;
        set => SetProperty(ref _selectedTime, value);
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
}