using System.ComponentModel.DataAnnotations;
using System.Globalization;
using DynamicData;
using SQLite;
using SQLiteNetExtensions.Attributes;
using MyTime.Core;

namespace MyTime.Model;

public class Employer : ControllerBase
{
    public Employer()
    {
        _id = String.Empty;
        _name = String.Empty;
        _employerNb = 0;
        _description = String.Empty;
        _salary = 0;
        _addressLine1 = String.Empty;
        _addressLine2 = String.Empty;
        _times = new SourceCache<Time, string>(t => t.Id);
    }

    public bool istString(double a)
    {

        return true;
    }

    public bool istString(int b, double c)
    {
        return false;
    }
    public bool istString(int b, float c)
    {
        return false;
    }
    

    public Employer(string id, string name, long employerNb = 0, string description = "", float salary = 0, string addressLine1 = "", string addressLine2 = "")
    {
        _id = id;
        _name = name;
        _employerNb = employerNb;
        _description = description;
        _salary = salary;
        _addressLine1 = addressLine1;
        _addressLine2 = addressLine2;
        _times = new SourceCache<Time, string>(t => t.Id);
    }
    
    [Key]
    private string _id;

    [PrimaryKey]
    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
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
    private string _description;

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
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

    public static string getUUID()
    {
        Guid myuuid = Guid.NewGuid();
        return myuuid.ToString();
    }

    private SourceCache<Time, string> _times;
    [OneToMany]
    public SourceCache<Time, string> Times
    {
        get => _times;
        set => SetProperty(ref _times, value);
    }
}