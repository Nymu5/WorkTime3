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

public class EmployerController : ReactiveObject
{
    public EmployerController()
    {
        CreateEmployer = ReactiveCommand.CreateFromTask(AddEmployerTask);
        RefreshCommand = ReactiveCommand.CreateFromTask(RefreshTask);
        SelectionChangedCommand = ReactiveCommand.CreateFromTask(SelectionChangedTask);
        
        var disposable = Constants.Employers
            .Connect()
            .Sort(SortExpressionComparer<Employer>.Ascending(e => e.Name))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out Employers)
            .Subscribe();
    }

    public ICommand CreateEmployer { get; set; }
    public ICommand RefreshCommand { get; set; }
    public ICommand SelectionChangedCommand { get; set; }

    public readonly ReadOnlyObservableCollection<Employer> Employers;

    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => this.RaiseAndSetIfChanged(ref _isRefreshing, value);
    }

    private Employer _selectedEmployer;
    public Employer SelectedEmployer
    {
        get => _selectedEmployer;
        set => this.RaiseAndSetIfChanged(ref _selectedEmployer, value);
    }
    
    // Functions
    private async Task AddEmployerTask()
    {
        await Shell.Current.GoToAsync(nameof(AddEmployerPage));
    }

    private async Task RefreshTask()
    {
        Constants.Employers.AddOrUpdate(await Constants.Database.GetEmployersAsync());
        IsRefreshing = false;
    }

    private async Task SelectionChangedTask()
    {
        if (SelectedEmployer != null)
        {
            await Shell.Current.GoToAsync("DetailEmployerPage", new Dictionary<string, object>
            {
                { "Employer", _selectedEmployer },
                { "EmployerId", _selectedEmployer.Id }
            });
            SelectedEmployer = null;
        }
    }
}