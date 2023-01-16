using System;
using System.Threading.Tasks;
using MyTime.Core;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace MyTime.Core.WT3Core
{
    [Preserve(AllMembers = true)]
    [Table("Item")]
    public class Item : ControllerBase
    {
        [PrimaryKey, Column("_id")]
        public string Id { get; set; }
        [ForeignKey(typeof(Employer))]
        public string EmployerId { get; set; }
        private Employer employer;
        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Employer Employer
        {
            get => employer;
            set => SetProperty(ref employer, value);
        }

        private string employerString;
        public string EmployerString { get => employerString; set => SetProperty(ref employerString, value); }

        [Ignore]
        public string EmployerStringDisplay
        {
            get => Employer != null ? Employer.Name : EmployerString + " [Deleted]";
        }
        private string text;
        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        private string description;

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        private DateTime startDate;

        public DateTime StartDate
        {
            get => startDate;
            set
            {
                SetProperty(ref startDate, value);
            } 
        }

        public TimeSpan StartTime
        {
            get => new TimeSpan(StartDate.Hour, StartDate.Minute, 0);
            set
            {
                SetProperty(ref startDate,
                    new DateTime(startDate.Year, startDate.Month, startDate.Day, value.Hours, value.Minutes, 0));
            }
        }

        private DateTime endDate;

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                SetProperty(ref endDate, value);
                OnPropertyChanged(nameof(EndDateStr));
            }
        }
        public TimeSpan EndTime
        {
            get => new TimeSpan(EndDate.Hour, EndDate.Minute, 0);
            set
            {
                SetProperty(ref endDate,
                    new DateTime(endDate.Year, endDate.Month, endDate.Day, value.Hours, value.Minutes, 0));
            }
        }
        public string EndDateStr
        {
            get => endDate.ToString("dd.MM.yyyy HH:mm");
        }
        
        private double salary;
        public double Salary { get => salary; set => SetProperty(ref salary, Math.Round(value, 2)); }

        private bool _onInvoice;

        public bool OnInvoice
        {
            get => _onInvoice;
            set => SetProperty(ref _onInvoice, value);
        }
    }
}
