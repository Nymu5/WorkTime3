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

        async void SaveTimeTask(bool canSave)
        {
            Time.Id ??= Time.GetUuid();
            //if (Time.Id == null) Time.Id = Time.getUUID();
            Employer employer = Constants.Employers.Lookup(SelectedEmployer.Id).Value;
            Time.Employer = employer;
            await Constants.Database.SaveTimeAsync(Time, true);
            employer.Times.Add(Time);
            await Constants.Database.SaveEmployerAsync(Time.Employer);
            await Shell.Current.GoToAsync("..");
        }

        SaveTimeCommand = new Command<bool>(execute: SaveTimeTask, canExecute: (canSave) => canSave);
        
        async void DeleteTimeTask(bool canDelete)
        {
            var result = await Shell.Current.DisplayActionSheet($"Are you sure you want to delete this entry?", "Cancel", "Yes");
            if (result != "Yes") return;
            await Constants.Database.DeleteTimeAsync(Time);
            await Shell.Current.GoToAsync("..");
        }

        DeleteTimeCommand = new Command<bool>(execute: DeleteTimeTask, canExecute: (canDelete) => canDelete);

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

        Constants.Employers
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
    public readonly ReadOnlyObservableCollection<Employer> Employers;

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

    private readonly Time _time;
    public Time Time
    {
        get => _time;
        init
        {
            this.RaiseAndSetIfChanged(ref _time, value);
            this.RaisePropertyChanged(nameof(CanDelete));
            this.RaisePropertyChanged(nameof(StartDate));
            this.RaisePropertyChanged(nameof(EndDate));
            this.RaisePropertyChanged(nameof(StartTime));
            this.RaisePropertyChanged(nameof(EndTime));
        } 
    }

    public DateTime StartDate
    {
        get => Time.Start.Date;
        set
        {
            int aYears = value.Year - Time.Start.Year;
            int aMonths = value.Month - Time.Start.Month;
            int aDays = value.Day - Time.Start.Day;
            Time.Start = Time.Start.AddYears(aYears);
            Time.Start = Time.Start.AddMonths(aMonths);
            Time.Start = Time.Start.AddDays(aDays);
            Time.End = Time.End.AddYears(aYears);
            Time.End = Time.End.AddMonths(aMonths);
            Time.End = Time.End.AddDays(aDays);
            this.RaisePropertyChanged(nameof(EndDate));
            this.RaisePropertyChanged(nameof(CanSave));
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
            this.RaisePropertyChanged(nameof(CanSave));
        }
    }

    public TimeSpan StartTime
    {
        get => Time.Start.TimeOfDay;
        set
        {
            int aHours = value.Hours - Time.Start.Hour;
            int aMinutes = value.Minutes - Time.Start.Minute;
            Time.Start = Time.Start.AddHours(aHours);
            Time.Start = Time.Start.AddMinutes(aMinutes);
            Time.End = Time.End.AddHours(aHours);
            Time.End = Time.End.AddMinutes(aMinutes);
            this.RaisePropertyChanged(nameof(EndTime));
            this.RaisePropertyChanged(nameof(EndDate));
            this.RaisePropertyChanged(nameof(CanSave));
        }
    }

    public TimeSpan EndTime
    {
        get => Time.End.TimeOfDay;
        set
        {
            Time.End = Time.End.AddHours(value.Hours - Time.End.Hour);
            Time.End = Time.End.AddMinutes(value.Minutes - Time.End.Minute);
            this.RaisePropertyChanged(nameof(CanSave));
        }
    }
    
    public bool CanSave => SelectedEmployer != null && Time.Start < Time.End;

    public bool CanDelete => !String.IsNullOrWhiteSpace(Time.Id);

    // Functions
}