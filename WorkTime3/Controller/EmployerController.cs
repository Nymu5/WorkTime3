using System.Windows.Input;
using WorkTime3.Core;

namespace WorkTime3.Controller;

public class EmployerController : ControllerBase
{
    public EmployerController()
    {
        CreateEmployer = new Command(() =>
        {
            Shell.Current.GoToAsync("AddEmployerPage");
            Console.WriteLine("Add clicked!");
        }, () => true);
    }

    public ICommand CreateEmployer { get; set; }
}