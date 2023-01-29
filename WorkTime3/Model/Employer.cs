using System.ComponentModel.DataAnnotations;
using SQLite;
using SQLiteNetExtensions.Attributes;
using ReactiveUI;

namespace MyTime.Model;

public class Employer : ReactiveObject, IComparable
{
    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        if (obj is Employer oEmployer) return String.Compare(this.Id, oEmployer.Id, StringComparison.Ordinal);
        else throw new ArgumentException("Object is not an Employer");
    }

    public bool Equals(Employer obj)
    {
        return String.Equals(this.Id, obj.Id, StringComparison.Ordinal);
    }
    
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

    [Key] private readonly string _id;

    [PrimaryKey]
    public string Id
    {
        get => _id;
        init => this.RaiseAndSetIfChanged(ref _id, value);
    }

    private readonly string _name;

    public string Name
    {
        get => _name;
        init => this.RaiseAndSetIfChanged(ref _name, String.IsNullOrWhiteSpace(value) ? null : value);
    }

    private readonly long _employerNb;

    public long EmployerNb
    {
        get => _employerNb;
        init => this.RaiseAndSetIfChanged(ref _employerNb, value);
    }

    private readonly string _description;

    public string Description
    {
        get => _description;
        init => this.RaiseAndSetIfChanged(ref _description, value);
    }

    private readonly double _salary;

    public double Salary
    {
        get => _salary;
        init => this.RaiseAndSetIfChanged(ref _salary, (float)Math.Round(value, 2));
    }

    public string SalaryString => Salary.ToString("C");
    private readonly string _addressLine1;

    public string AddressLine1
    {
        get => _addressLine1;
        init => this.RaiseAndSetIfChanged(ref _addressLine1, value);
    }

    private readonly string _addressLine2;

    public string AddressLine2
    {
        get => _addressLine2;
        init => this.RaiseAndSetIfChanged(ref _addressLine2, value);
    }

    public static string GetUuid()
    {
        return Guid.NewGuid().ToString();
    }

    private readonly List<Time> _times;

    [OneToMany(CascadeOperations = CascadeOperation.CascadeDelete)]
    public List<Time> Times
    {
        get => _times;
        init => this.RaiseAndSetIfChanged(ref _times, value);
    }

    [Ignore] public string EmployerDetailString => $"{Name}\n{AddressLine1}\n{AddressLine2}\n\nEmployer #\n{EmployerNb}";
}