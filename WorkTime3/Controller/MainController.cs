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
            Employers = await _db.GetEmployersAsync();
            Times = await _db.GetTimesAsync(); 
            
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
    
    private SourceCache<Time, string> _times;
    public SourceCache<Time, string> Times
    {
        get => _times;
        set => SetProperty(ref _times, value);
    }
    
    // Functions
}