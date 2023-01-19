using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;

namespace MyTime.Controller;

public class TimeController : ReactiveObject
{
    public TimeController()
    {
        CreateTimeCommand = ReactiveCommand.CreateFromTask(CreateTimeTask);
        SelectionChangedCommand = ReactiveCommand.CreateFromTask(SelectionChangedTask);

        Func<Time, bool> SearchTermFilter(string text) => time => String.IsNullOrWhiteSpace(text) ||
                                                                  time.Text.ToLower().Contains(text.ToLower()) ||
                                                                  time.Employer.Name.ToLower().Contains(text.ToLower());

        var filterPredicate = this.WhenAnyValue(x => x.SearchTerm)
            .Throttle(TimeSpan.FromMilliseconds(250), RxApp.TaskpoolScheduler)
            .DistinctUntilChanged()
            .Select(SearchTermFilter);

        var disposable = Constants.Times
            .Connect()
            .Filter(filterPredicate)
            .Sort(SortExpressionComparer<Time>.Descending(t => t.Start))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out Times)
            .Subscribe();
    }

    // Commands
    public ICommand CreateTimeCommand { get; }
    public ICommand SelectionChangedCommand { get; }

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
    
    // Functions
    private async Task CreateTimeTask()
    {
        await Shell.Current.GoToAsync("AddTimePage");
    }

    private async Task SelectionChangedTask()
    {
        if (SelectedTime != null)
        {
            await Shell.Current.GoToAsync("AddTimePage", new Dictionary<string, object>
            {
                { "Time", SelectedTime },
            });
            SelectedTime = null;
        }
    }
}