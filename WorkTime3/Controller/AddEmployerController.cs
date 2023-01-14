using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;
using MyTime.View;

namespace MyTime.Controller;

[QueryProperty(nameof(Employer), "Employer")]

public class AddEmployerController : ControllerBase
{
    private MyTimeDatabase _db;
    private string _id = null; 
    public AddEmployerController()
    {
        _db = new MyTimeDatabase();
        SaveEmployerCommand = new Command(
            execute: async () =>
            {
                Employer employer;
                if (Employer == null)
                {
                    employer = new Employer();
                    if (_id == null)
                    {
                        do
                        {
                            employer.Id = Employer.getUUID();
                        } while (await _db.GetEmployerAsync(employer.Id) != null); 
                    }
                }
                else
                {
                    employer = Employer;
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

    private Employer _employer;

    public Employer Employer
    {
        get => _employer;
        set
        {
            SetProperty(ref _employer, value);
            Name = Employer.Name;
            EmployerNb = Employer.EmployerNb;
            Salary = Employer.Salary;
            AddressLine1 = Employer.AddressLine1;
            AddressLine2 = Employer.AddressLine2;
            Description = Employer.Description;
        }
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