using System.Collections.ObjectModel;
using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;
using MyTime.View;

namespace MyTime.Controller;

public class EmployerController : ControllerBase
{
    private MyTimeDatabase _db;

    public EmployerController()
    {
        _db = new MyTimeDatabase();
        CreateEmployer = new Command(() =>
        {
            //Shell.Current.GoToAsync(nameof(AddEmployerPage));
            Shell.Current.Navigation.PushAsync(new AddEmployerPage());
            Console.WriteLine("Add clicked!");
        }, () => true);
        RefreshCommand = new Command(execute: async () =>
        {
            Employers = await _db.GetEmployersAsync();
            IsRefreshing = false;
        });
        SelectionChangedCommand = new Command(execute: () =>
        {
            if (SelectedEmployer != null)
            {
                Shell.Current.GoToAsync("DetailEmployerPage", new Dictionary<string, object>
                {
                    { "Employer", _selectedEmployer },
                    { "EmployerId", _selectedEmployer.Id }
                });
                SelectedEmployer = null;
            }
        }, canExecute: () => true);
    }

    public ICommand CreateEmployer { get; set; }
    public ICommand RefreshCommand { get; set; }
    public ICommand SelectionChangedCommand { get; set; }

    private List<Employer> _employers;

    public List<Employer> Employers
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
        set { SetProperty(ref _selectedEmployer, value); }
    }
}