using ReactiveUI;
using SQLite;
// ReSharper disable All

namespace MyTime.Core.WT3Core
{
    [Preserve(AllMembers = true)]
    [Table("Employers")]
    public class Employer : ReactiveObject
    {
        [PrimaryKey, Column("_id")] public string Id { get; set; }

        private string name;

        public string Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value);
        }

        private string address;

        public string Address
        {
            get => address;
            set => this.RaiseAndSetIfChanged(ref address, value);
        }

        private string description;

        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }

        private double salary = 10.45;

        public double Salary
        {
            get => salary;
            set => this.RaiseAndSetIfChanged(ref salary, value);
        }

        private string _employerNumber;

        public string EmployerNumber
        {
            get => _employerNumber;
            set => this.RaiseAndSetIfChanged(ref _employerNumber, value);
        }

        public DateTime DateCreated { get; set; }

        public Employer()
        {
        }

        public Employer(Employer employer)
        {
            Id = employer.Id;
            name = employer.Name;
            address = employer.Address;
            description = employer.Description;
            salary = employer.Salary;
            _employerNumber = employer._employerNumber;
            DateCreated = employer.DateCreated;
        }
    }
}