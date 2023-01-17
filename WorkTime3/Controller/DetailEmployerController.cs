using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;

namespace MyTime.Controller;

[QueryProperty(nameof(Employer), "Employer")]
[QueryProperty(nameof(EmployerId), "EmployerId")]
public class DetailEmployerController : ControllerBase
{
    private MyTimeDatabase _db;

    public DetailEmployerController()
    {
        _db = new MyTimeDatabase();
        EditEmployerCommand = new Command(canExecute: () => true, execute: () =>
        {
            if (Employer == null) return;
            Shell.Current.GoToAsync("AddEmployerPage", new Dictionary<string, object>
            {
                { "Employer", Employer },
            });
        });
        DeleteEmployerCommand = new Command(canExecute: () => true, execute: async () =>
        {
            var result = await Shell.Current.DisplayActionSheet(
                $"Are you sure you want to delete {Employer.Name} and ALL OF THE TRACKED HOURS? This operation cannot be undone!",
                "Cancel", "Yes");
            if (result != "Yes") return;
            await _db.DeleteEmployerAsync(Employer);
            await Shell.Current.GoToAsync("..");
        });
    }

    private Employer _employer;

    public Employer Employer
    {
        get => _employer;
        set => SetProperty(ref _employer, value);
    }

    private string _employerId;

    public string EmployerId
    {
        get => _employerId;
        set
        {
            SetProperty(ref _employerId, value);
            Console.WriteLine($"HALLLOOOOOOOOOOO: {_employer}");
        }
    }

    public ICommand EditEmployerCommand { get; set; }
    public ICommand DeleteEmployerCommand { get; set; }
}