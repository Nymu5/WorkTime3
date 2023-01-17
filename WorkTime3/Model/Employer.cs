using System.ComponentModel.DataAnnotations;
using System.Globalization;
using SQLite;
using SQLiteNetExtensions.Attributes;
using MyTime.Core;

namespace MyTime.Model;

public class Employer : ControllerBase
{
    public Employer()
    {
        _id = String.Empty;
        _name = null;
        _employerNb = 0;
        _description = String.Empty;
        _salary = 0;
        _addressLine1 = String.Empty;
        _addressLine2 = String.Empty;
        _times = new List<Time>();
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


    public Employer(string id, string name, long employerNb = 0, string description = "", float salary = 0,
        string addressLine1 = "", string addressLine2 = "")
    {
        _id = id;
        _name = name;
        _employerNb = employerNb;
        _description = description;
        _salary = salary;
        _addressLine1 = addressLine1;
        _addressLine2 = addressLine2;
        _times = new List<Time>();
    }

    [Key] private string _id;

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
        set => SetProperty(ref _name, String.IsNullOrWhiteSpace(value) ? null : value);
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

    private double _salary;

    public double Salary
    {
        get => _salary;
        set => SetProperty(ref _salary, (float)Math.Round(value, 2));
    }

    public string SalaryString => Salary.ToString("C");
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

    private List<Time> _times;

    [OneToMany(CascadeOperations = CascadeOperation.CascadeDelete)]
    public List<Time> Times
    {
        get => _times;
        set => SetProperty(ref _times, value);
    }
}