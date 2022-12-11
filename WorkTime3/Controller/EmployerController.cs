using System.Windows.Input;
using WorkTime3.Core;
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
    }

    public ICommand CreateEmployer { get; set; }
    
}