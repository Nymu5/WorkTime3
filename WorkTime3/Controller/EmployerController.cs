using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using WorkTime3.Core;
using WorkTime3.Model;
using WorkTime3.View;

namespace WorkTime3.Controller;

public class EmployerController : ControllerBase
{
    private WorkTime3Database _db;
    public EmployerController()
    {
        _db = new WorkTime3Database();
        CreateEmployer = new Command(() =>
        {
            //Shell.Current.GoToAsync(nameof(AddEmployerPage));
            Shell.Current.Navigation.PushAsync(new AddEmployerPage());
            Console.WriteLine("Add clicked!");
        }, () => true);
        RefreshCommand = new Command(execute: async () =>
        {
            SourceCache<Employer, string> employers = await _db.GetEmployersAsync();
            var disposable = employers
                .Connect()
                .Sort(SortExpressionComparer<Employer>.Ascending(e => e.Name))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _employers)
                .DisposeMany()
                .Subscribe();
            OnPropertyChanged(nameof(Employers));
            IsRefreshing = false;
        }, canExecute: () =>
        {
            return true;
        });
        SelectionChangedCommand = new Command(execute: () =>
        {
            if (SelectedEmployer != null)
            {
                Shell.Current.GoToAsync("DetailEmployerPage", new Dictionary<string, object>
                {
                    {"Employer", _selectedEmployer},
                    {"EmployerId", _selectedEmployer.Id}
                });
                SelectedEmployer = null;
            }
        }, canExecute: () => true);
    }

    public ICommand CreateEmployer { get; set; }
    public ICommand RefreshCommand { get; set; }
    public ICommand SelectionChangedCommand { get; set; }

    private ReadOnlyObservableCollection<Employer> _employers;
    public ReadOnlyObservableCollection<Employer> Employers
    {
        get => _employers;
        set => SetProperty(ref _employers, value);
    }


    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    private Employer _selectedEmployer;
    public Employer SelectedEmployer
    {
        get => _selectedEmployer;
        set
        {
            SetProperty(ref _selectedEmployer, value);
        }
    }
}