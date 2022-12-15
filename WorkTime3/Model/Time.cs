using WorkTime3.Core;

namespace WorkTime3.Model;

public class Time : ControllerBase
{
    private WorkTime3Database Database;

    public Time()
    {
        Database = new WorkTime3Database();
    }

    private string _id;
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private string _employerId;
    public string EmployerId
    {
        get => _employerId;
        set => SetProperty(ref _employerId, value);
    }
    
    private string _employerName;
    public string EmployerName
    {
        get
        {
            if (!String.IsNullOrWhiteSpace(_employerId) && Database.GetEmployerAsync(_employerId).Result != null)
            {
                return Database.GetEmployerAsync(_employerId).Result.Name;
            }

            return _employerName;
        }
        set => SetProperty(ref _employerName, value);
    }

    public Employer Employer
    {
        get => Database.GetEmployerAsync(_employerId).Result;
        set
        {
            SetProperty(ref _employerId, value.Id);
            SetProperty(ref _employerName, value.Name);
        }
    }

    private string _text;
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
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
    
    public static string getUUID()
    {
        Guid myuuid = Guid.NewGuid();
        return myuuid.ToString();
    }
}