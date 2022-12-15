using WorkTime3.Core;
using WorkTime3.Model;

namespace WorkTime3.Controller;

[QueryProperty(nameof(Employer), "Employer")]
[QueryProperty(nameof(EmployerId), "EmployerId")]
public class DetailEmployerController : ControllerBase
{
    public DetailEmployerController()
    {
        
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
}