using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using DynamicData;
using DynamicData.Binding;
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

        LoadCommand = new Command(execute: () =>
        {
            if (Time.Employer != null) SelectedEmployer = Constants.Employers.Lookup(Time.Employer.Id).Value;
        });

        SaveTimeCommand = new Command<bool>(execute: async (canSave) =>
        {
            Time.Id ??= Time.getUUID();
            //if (Time.Id == null) Time.Id = Time.getUUID();
            Employer employer = Constants.Employers.Lookup(SelectedEmployer.Id).Value;
            Time.Employer = employer;
            await Constants.Database.SaveTimeAsync(Time);
            employer.Times.Add(Time);
            await Constants.Database.SaveEmployerAsync(Time.Employer);
            await Shell.Current.GoToAsync("..");
        }, canExecute: (canSave) => canSave);

        var canDelete = this.WhenAnyValue(x => x.CanDelete);
        DeleteTimeCommand = new Command<bool>(execute: async (canDelete) =>
        {
            var result = await Shell.Current.DisplayActionSheet($"Are you sure you want to delete this entry?",
                "Cancel", "Yes");
            if (result != "Yes") return;
            await Constants.Database.DeleteTimeAsync(Time);
            await Shell.Current.GoToAsync("..");
        }, canExecute: (canDelete) => canDelete);

        UpdateSalaryCommand = new Command(execute: () =>
        {
            if (Time.Id == null) Time.Salary = SelectedEmployer.Salary;
        });
        PickerChangedCommand = new Command(execute: () =>
        {
            if (Time.Employer != null && Employers != null)
            {
                SelectedEmployer = Employers.FirstOrDefault(e => e.Id == Time.Employer.Id); 
                this.RaisePropertyChanged(nameof(SelectedEmployer));
            }
        });

        var disposable = Constants.Employers
            .Connect()
            .Sort(SortExpressionComparer<Employer>.Ascending(e => e.Name))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out Employers)
            .Subscribe();
    }

    // Controls 
    public ICommand LoadCommand { get; }
    public ICommand SaveTimeCommand { get; }
    public ICommand UpdateSalaryCommand { get; }
    public ICommand DeleteTimeCommand { get; }
    public ICommand PickerChangedCommand { get; }

    // Properties
    public ReadOnlyObservableCollection<Employer> Employers;

    private Employer _selectedEmployer;
    public Employer SelectedEmployer
    {
        get => _selectedEmployer;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedEmployer, value);
            this.RaisePropertyChanged(nameof(CanSave));
        } 
    }

    private Time _time;
    public Time Time
    {
        get => _time;
        set
        {
            this.RaiseAndSetIfChanged(ref _time, value);
            this.RaisePropertyChanged(nameof(CanDelete));
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
    
    public bool CanSave => SelectedEmployer != null;

    public bool CanDelete => !String.IsNullOrWhiteSpace(Time.Id);

    // Functions
}