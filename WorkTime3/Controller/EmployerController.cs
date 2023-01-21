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

[QueryProperty(nameof(DeleteEmployer), "Delete")]
public class EmployerController : ReactiveObject
{
    public EmployerController()
    {
        CreateEmployer = new Command(execute: async () =>
        {
            await Shell.Current.GoToAsync(nameof(AddEmployerPage));
        });
        RefreshCommand = new Command(execute: async () =>
        {
            Constants.Employers.Clear();
            Constants.Employers.AddOrUpdate(await Constants.Database.GetEmployersAsync());
            IsRefreshing = false;
        });
        SelectionChangedCommand = new Command(execute: async () =>
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
        });
        
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
    
    private Employer _deleteEmployer;
    public Employer DeleteEmployer
    {
        get => _deleteEmployer;
        set => this.RaiseAndSetIfChanged(ref _deleteEmployer, value);
    }
    
    // Functions
}