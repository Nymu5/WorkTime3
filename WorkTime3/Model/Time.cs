using SQLite;
using SQLiteNetExtensions.Attributes;
using MyTime.Core;

namespace MyTime.Model;

public class Time : ControllerBase
{
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