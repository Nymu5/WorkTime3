using System.Windows.Input;
using WorkTime3.Core;
using WorkTime3.Model;
using WorkTime3.View;

namespace WorkTime3.Controller;

public class AddEmployerController : ControllerBase
{
    private WorkTime3Database _db;
    private string _id = null; 
    public AddEmployerController()
    {
        _db = new WorkTime3Database();
        TestCommand = new Command(
            execute: () =>
            {
                Shell.Current.GoToAsync(nameof(AddTimePage)); 
            }, canExecute: () => true);
        SaveEmployerCommand = new Command(
            execute: async () =>
            {
                Employer employer = new Employer();
                if (_id == null)
                {
                    do
                    {
                        employer.Id = Employer.getUUID();
                    } while (await _db.GetEmployerAsync(employer.Id) != null); 
                }
                employer.Name = Name;
                employer.EmployerNb = EmployerNb;
                employer.Salary = Salary;
                employer.AddressLine1 = AddressLine1;
                employer.AddressLine2 = AddressLine2;
                employer.Description = Description;
                await _db.SaveEmployerAsync(employer);
                await Shell.Current.GoToAsync("..");
            }, canExecute: () =>
            {
                return true;
            });
    }
    private string _title = "Add Employer";
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    public ICommand TestCommand { get; set; }
    public ICommand SaveEmployerCommand { get; set; }

    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    private long _employerNb;
    public long EmployerNb
    {
        get => _employerNb;
        set => SetProperty(ref _employerNb, value);
    }

    private float _salary;
    public float Salary
    {
        get => _salary;
        set => SetProperty(ref _salary, value);
    }

    private string _addressLine1;
    public string AddressLine1
    {
        get => _addressLine1;
        set => SetProperty(ref _addressLine1, value);
    }

    private string _addressLine2;
    public string AddressLine2
    {
        get => _addressLine2;
        set => SetProperty(ref _addressLine2, value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
}