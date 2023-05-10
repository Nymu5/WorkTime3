using SQLite;
using SQLiteNetExtensions.Attributes;
using ReactiveUI;

namespace MyTime.Model;

public class Time : ReactiveObject
{
    public Time()
    {
        DateTime now = DateTime.Now;
        Start = new DateTime(now.Year, now.Month, now.Day, now.Hour, (now.Minute / 15) * 15, 0);
        End = (new DateTime(now.Year, now.Month, now.Day, now.Hour, (now.Minute / 15) * 15, 0)).AddHours(2);
    }

    private string _id;

    [PrimaryKey]
    public string Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private string _employerId;

    [ForeignKey(typeof(Employer))]
    public string EmployerId
    {
        get => _employerId;
        set => this.RaiseAndSetIfChanged(ref _employerId, value);
    }


    private Employer _employer;

    [ManyToOne(CascadeOperations = CascadeOperation.All)]
    public Employer Employer
    {
        get => _employer;
        set => this.RaiseAndSetIfChanged(ref _employer, value);
    }

    private string _text;

    public string Text
    {
        get => _text;
        set => this.RaiseAndSetIfChanged(ref _text, String.IsNullOrWhiteSpace(value) ? null : value);
    }

    private string _description;

    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    private DateTime _start;

    public DateTime Start
    {
        get => _start;
        set => this.RaiseAndSetIfChanged(ref _start, value);
    }

    private DateTime _end;

    public DateTime End
    {
        get => _end;
        set => this.RaiseAndSetIfChanged(ref _end, value);
    }

    private double _salary;

    public double Salary
    {
        get => _salary;
        set => this.RaiseAndSetIfChanged(ref _salary, (float)Math.Round(value, 2));
    }

    [Ignore] public string SalaryFloat => Salary.ToString("C");
    [Ignore] public string EarnedString => (Salary * Duration.TotalHours).ToString("C");

    public static string GetUuid()
    {
        return Guid.NewGuid().ToString();
    }

    [Ignore] public TimeSpan Duration => End - Start;
    [Ignore] public string DurationString => $"{(int)Duration.TotalHours}:{Duration.Minutes:00} h";
    [Ignore] public string TimeStartString => $"{Start:d} {Start:t}";
    [Ignore] public string TimeEndString => $"{End:d} {End:t}";

    [Ignore] public double Earned => Duration.TotalHours * Salary;
    
    private bool _isVisible = true;
    [Ignore]
    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }
}