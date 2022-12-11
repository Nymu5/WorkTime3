using System.Windows.Input;
using WorkTime3.Core;
using WorkTime3.View;

namespace WorkTime3.Controller;

public class AddEmployerController : ControllerBase
{
    private WorkTime3Database _db;
    public AddEmployerController()
    {
        _db = new WorkTime3Database();
        TestCommand = new Command(
            execute: () =>
            {
                Shell.Current.GoToAsync(nameof(AddTimePage)); 
            }, canExecute: () => true);
    }
    private string _title = "Add Employer";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    public ICommand TestCommand { get; set; }
}