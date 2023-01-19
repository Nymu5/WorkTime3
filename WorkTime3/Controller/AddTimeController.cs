using System.Collections.ObjectModel;
using System.Windows.Input;
using DynamicData;
using MyTime.Core;
using MyTime.Model;
using ReactiveUI;

namespace MyTime.Controller;

[QueryProperty(nameof(Time), "Time")]
public class AddTimeController : ReactiveObject
{
    public AddTimeController()
    {
        Time = new Time();

        LoadCommand = ReactiveCommand.CreateFromTask(LoadTask);

        var canSave = this.WhenAnyValue(x => x.CanSave);
        SaveTimeCommand = ReactiveCommand.CreateFromTask(SaveTask, canSave);

        var canDelete = this.WhenAnyValue(x => x.CanDelete);
        DeleteTimeCommand = ReactiveCommand.CreateFromTask(DeleteTask, canDelete);

        UpdateSalaryCommand = ReactiveCommand.CreateFromTask(UpdateSalaryTask);
    }

    // Controls 
    public ICommand LoadCommand { get; }
    public ICommand SaveTimeCommand { get; }
    public ICommand UpdateSalaryCommand { get; }
    public ICommand DeleteTimeCommand { get; }

    // Properties
    private Employer _selectedEmployer;
    public Employer SelectedEmployer
    {
        get => _selectedEmployer;
        set => this.RaiseAndSetIfChanged(ref _selectedEmployer, value);
    }

    private Time _time;
    public Time Time
    {
        get => _time;
        set => this.RaiseAndSetIfChanged(ref _time, value);
    }

    public DateTime StartDate
    {
        get => Time.Start.Date;
        set
        {
            Time.Start = Time.Start.AddYears(value.Year - Time.Start.Year);
            Time.Start = Time.Start.AddMonths(value.Month - Time.Start.Month);
            Time.Start = Time.Start.AddDays(value.Day - Time.Start.Day);
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
        }
    }

    public TimeSpan StartTime
    {
        get => Time.Start.TimeOfDay;
        set
        {
            Time.Start = Time.Start.AddHours(value.Hours - Time.Start.Hour);
            Time.Start = Time.Start.AddMinutes(value.Minutes - Time.Start.Minute);
        }
    }

    public TimeSpan EndTime
    {
        get => Time.End.TimeOfDay;
        set
        {
            Time.End = Time.End.AddHours(value.Hours - Time.End.Hour);
            Time.End = Time.End.AddMinutes(value.Minutes - Time.End.Minute);
        }
    }

    private bool _canSave;

    public bool CanSave
    {
        get => SelectedEmployer != null;
        set => this.RaiseAndSetIfChanged(ref _canSave, value);
    }

    private bool _canDelete = true;

    public bool CanDelete
    {
        get => _canDelete;
        set => this.RaiseAndSetIfChanged(ref _canDelete, value);
    }
    
    // Functions
    private async Task SaveTask()
    {
        Time.Id ??= Time.getUUID();
        //if (Time.Id == null) Time.Id = Time.getUUID();
        Employer employer = Constants.Employers.Lookup(SelectedEmployer.Id).Value;
        Time.Employer = employer;
        await Constants.Database.SaveTimeAsync(Time);
        employer.Times.Add(Time);
        await Constants.Database.SaveEmployerAsync(Time.Employer);
        await Shell.Current.GoToAsync("..");
    }

    private async Task DeleteTask()
    {
        var result = await Shell.Current.DisplayActionSheet($"Are you sure you want to delete this entry?",
            "Cancel", "Yes");
        if (result != "Yes") return;
        await Constants.Database.DeleteTimeAsync(Time);
        await Shell.Current.GoToAsync("..");
    }

    private async Task UpdateSalaryTask()
    {
        if (Time.Id == null) Time.Salary = SelectedEmployer.Salary;
    }

    private async Task LoadTask()
    {
        if (Time.Employer != null) SelectedEmployer = Constants.Employers.Lookup(Time.Employer.Id).Value;
    }
}