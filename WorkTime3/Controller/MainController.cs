using System.Windows.Input;
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
    private List<Employer> _employers;
    public List<Employer> Employers
    {
        get => _employers;
        set
        {
            SetProperty(ref _employers, value);
        } 
    }
    
    private List<Time> _times;
    public List<Time> Times
    {
        get => _times;
        set => SetProperty(ref _times, value);
    }
    
    // Functions
}