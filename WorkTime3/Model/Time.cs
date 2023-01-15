using SQLite;
using SQLiteNetExtensions.Attributes;
using MyTime.Core;

namespace MyTime.Model;

public class Time : ControllerBase
{
    private Profile _profile;
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
        set => SetProperty(ref _id, value);
    }

    private string _employerId;
    [ForeignKey(typeof(Employer))]
    public string EmployerId
    {
        get => _employerId;
        set => SetProperty(ref _employerId, value);
    }
    
    
    private Employer _employer;
    [ManyToOne(CascadeOperations = CascadeOperation.All)]
    public Employer Employer
    {
        get => _employer;
        set => SetProperty(ref _employer, value);
    }

    private string _text;
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, String.IsNullOrWhiteSpace(value) ? null : value);
    }

    private string _description;
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    private DateTime _start;
    public DateTime Start
    {
        get => _start;
        set => SetProperty(ref _start, value);
    }

    private DateTime _end;
    public DateTime End
    {
        get => _end;
        set => SetProperty(ref _end, value);
    }
    
    private float _salary;
    public float Salary
    {
        get => _salary;
        set => SetProperty(ref _salary, value);
    }

    [Ignore] public string SalaryFloat => Salary.ToString("C");
    [Ignore] public string EarnedString => (Salary * Duration.TotalHours).ToString("C");
    
    public static string getUUID()
    {
        Guid myuuid = Guid.NewGuid();
        return myuuid.ToString();
    }

    [Ignore] public TimeSpan Duration => End - Start;
    [Ignore] public string DurationString => $"{(int)Duration.TotalHours}:{Duration.Minutes.ToString("00")} h";
    [Ignore] public string TimeString => $"{Start.ToString("dd.MM.yyyy HH:mm")} - {End.ToString("dd.MM.yyyy HH:mm")}";
    [Ignore] public string TimeStartString => $"{Start.ToString("dd.MM.yyyy HH:mm")}";
    [Ignore] public string TimeEndString => $"{End.ToString("dd.MM.yyyy HH:mm")}";
}