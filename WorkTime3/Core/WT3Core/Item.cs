using ReactiveUI;
using SQLite;
using SQLiteNetExtensions.Attributes;
// ReSharper disable All

namespace MyTime.Core.WT3Core
{
    [Preserve(AllMembers = true)]
    [Table("Item")]
    public class Item : ReactiveObject
    {
        [PrimaryKey, Column("_id")] public string Id { get; set; }
        [ForeignKey(typeof(Employer))] public string EmployerId { get; set; }
        private Employer employer;

        [ManyToOne(CascadeOperations = CascadeOperation.All)]
        public Employer Employer
        {
            get => employer;
            set => this.RaiseAndSetIfChanged(ref employer, value);
        }

        private string employerString;

        public string EmployerString
        {
            get => employerString;
            set => this.RaiseAndSetIfChanged(ref employerString, value);
        }

        [Ignore]
        public string EmployerStringDisplay
        {
            get => Employer != null ? Employer.Name : EmployerString + " [Deleted]";
        }

        private string text;

        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }

        private string description;

        public string Description
        {
            get => description;
            set => this.RaiseAndSetIfChanged(ref description, value);
        }

        private DateTime startDate;

        public DateTime StartDate
        {
            get => startDate;
            set { this.RaiseAndSetIfChanged(ref startDate, value); }
        }

        public TimeSpan StartTime
        {
            get => new TimeSpan(StartDate.Hour, StartDate.Minute, 0);
            set
            {
                this.RaiseAndSetIfChanged(ref startDate,
                    new DateTime(startDate.Year, startDate.Month, startDate.Day, value.Hours, value.Minutes, 0));
            }
        }

        private DateTime endDate;

        public DateTime EndDate
        {
            get => endDate;
            set
            {
                this.RaiseAndSetIfChanged(ref endDate, value);
                this.RaisePropertyChanged(nameof(EndDateStr));
            }
        }

        public TimeSpan EndTime
        {
            get => new TimeSpan(EndDate.Hour, EndDate.Minute, 0);
            set
            {
                this.RaiseAndSetIfChanged(ref endDate,
                    new DateTime(endDate.Year, endDate.Month, endDate.Day, value.Hours, value.Minutes, 0));
            }
        }

        public string EndDateStr
        {
            get => endDate.ToString("dd.MM.yyyy HH:mm");
        }

        private double salary;

        public double Salary
        {
            get => salary;
            set => this.RaiseAndSetIfChanged(ref salary, Math.Round(value, 2));
        }

        private bool _onInvoice;

        public bool OnInvoice
        {
            get => _onInvoice;
            set => this.RaiseAndSetIfChanged(ref _onInvoice, value);
        }
    }
}