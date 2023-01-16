using System.Collections.ObjectModel;
using System.Windows.Input;
using MyTime.Core;
using MyTime.Model;

namespace MyTime.Controller;

[QueryProperty(nameof(Time), "Time")]
[QueryProperty(nameof(Employers), "Employers")]
public class AddTimeController : ControllerBase
{
    private MyTimeDatabase _db;
    public AddTimeController()
    {
        Time = new Time();
        _db = new MyTimeDatabase();

        LoadCommand = new Command(execute: async () =>
        {
            Employers = await _db.GetEmployersAsync();
            if (Time.Employer != null) SelectedEmployer = Employers.FirstOrDefault(e => e.Id == Time.Employer.Id); 
        });
        
        SaveTimeCommand = new Command<bool>(canExecute: (canSave) => canSave, execute: async (canSave) =>
        {
            if (Time.Id == null) Time.Id = Time.getUUID();
            Employer employer = await _db.GetEmployerAsync(SelectedEmployer.Id);
            Time.Employer = employer;
            await _db.SaveTimeAsync(Time);
            employer.Times.Add(Time);
            await _db.SaveEmployerAsync(Time.Employer);
            await Shell.Current.GoToAsync("..");
        });
        UpdateSalaryCommand = new Command(execute: () =>
        {
            Time.Salary = SelectedEmployer.Salary;
        });
        BindingChangedCommand = new Command(execute: () =>
        {
            
        });
        DeleteTimeCommand = new Command<bool>(canExecute: (canDelete) => canDelete, execute: async (canDelete) =>
        {
            var result = await Shell.Current.DisplayActionSheet($"Are you sure you want to delete this entry?",
                "Cancel", "Yes");
            if (result != "Yes") return;
            await _db.DeleteTimeAsync(Time);
            await Shell.Current.GoToAsync("..");
        });
    }
    
    // Controls 
    public ICommand LoadCommand { get; }
    public ICommand SaveTimeCommand { get; }
    public ICommand UpdateSalaryCommand { get; }
    public ICommand BindingChangedCommand { get; }
    public ICommand DeleteTimeCommand { get; }
    
    // Properties
    private List<Employer> _employers;
    public List<Employer> Employers
    {
        get => _employers;
        set => SetProperty(ref _employers, value);
    }
    
    private Employer _selectedEmployer;
    public Employer SelectedEmployer
    {
        get => _selectedEmployer;
        set
        {
            SetProperty(ref _selectedEmployer, value);
            OnPropertyChanged(nameof(CanSave));
        }
    }
    
    private Time _time;
    public Time Time
    {
        get => _time;
        set
        {
            SetProperty(ref _time, value);
            Console.WriteLine(Time.Employer);
        } 
    }

    public DateTime StartDate
    {
        get => Time.Start.Date;
        set
        {
            Time.Start = Time.Start.AddYears(value.Year - Time.Start.Year);
            Time.Start = Time.Start.AddMonths(value.Month - Time.Start.Month);
            Time.Start = Time.Start.AddDays(value.Day - Time.Start.Day);
            OnPropertyChanged(nameof(Time.Start));
        }
    }

    public DateTime EndDate
    {
        get => Time.End.Date;
        set
        {
            Time.End = Time.End.AddYears(value.Year - Time.End.Year);
            Time.End = Time.End.AddMonths(value.Month - Time.End.Month);
            Time.End = Time.End.AddDays(value.Day - Time.End.Day);
            OnPropertyChanged(nameof(Time.End));
        }
    }

    public TimeSpan StartTime
    {
        get => Time.Start.TimeOfDay;
        set
        {
            Time.Start = Time.Start.AddHours(value.Hours - Time.Start.Hour);
            Time.Start = Time.Start.AddMinutes(value.Minutes - Time.Start.Minute);
            OnPropertyChanged(nameof(Time.Start));
        }
    }

    public TimeSpan EndTime
    {
        get => Time.End.TimeOfDay;
        set
        {
            Time.End = Time.End.AddHours(value.Hours - Time.End.Hour);
            Time.End = Time.End.AddMinutes(value.Minutes - Time.End.Minute);
            OnPropertyChanged(nameof(Time.End));
        }
    }

    private bool _canSave;
    public bool CanSave
    {
        get => SelectedEmployer != null;
        set => SetProperty(ref _canSave, value);
    }
    
    private bool _canDelete = true;
    public bool CanDelete
    {
        get => _canDelete;
        set => SetProperty(ref _canDelete, value);
    }
}