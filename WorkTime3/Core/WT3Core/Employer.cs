using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MyTime.Core;
using SQLite;

namespace MyTime.Core.WT3Core
{
    [Preserve(AllMembers = true)]
    [Table("Employers")]
    public class Employer : ControllerBase
    {
        [PrimaryKey, Column("_id")] public string Id { get; set; }

        private string name;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string address;

        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }

        private string description;

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        private double salary = 10.45;

        public double Salary
        {
            get => salary;
            set => SetProperty(ref salary, value);
        }

        private string _employerNumber;

        public string EmployerNumber
        {
            get => _employerNumber;
            set => SetProperty(ref _employerNumber, value);
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