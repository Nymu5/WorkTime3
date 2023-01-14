using System.Windows.Input;
using DynamicData;
using MyTime.Core;
using MyTime.Model;

namespace MyTime.Controller;

public class MainController : ControllerBase
{
    private MyTimeDatabase _db = new MyTimeDatabase(); 
    public MainController()
    {
        LoadDashboardCommand = new Command(execute: async () =>
        {
            SourceCache<Employer, string> Employers = await _db.GetEmployersAsync();
            EmployersCount = Employers.Count;
        });
    }

    // Commands
    public ICommand LoadDashboardCommand { get; }
    
    // Properties
    private SourceCache<Employer, string> _employers;
    public SourceCache<Employer, string> Employers
    {
        get => _employers;
        set
        {
            SetProperty(ref _employers, value);
        } 
    }

    private int _employersCount;
    public int EmployersCount
    {
        get => _employersCount;
        set => SetProperty(ref _employersCount, value);
    }
    
    // Functions
}