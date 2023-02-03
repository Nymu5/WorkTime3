using System.Reactive.Disposables;
using ReactiveUI;

namespace MyTime.View;

public partial class InvoiceCreatorPage
{
    public InvoiceCreatorPage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, x => x.Employer.EmployerDetailString, x => x.EmployerDetailString.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.Settings.PersonalDetailString, x => x.PersonalDetailString.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.Settings.BankDetailString, x => x.BankDetailString.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.Start, x => x.StartDate.Date)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.End, x => x.EndDate.Date)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.OneItem, x => x.OneElementCheckbox.IsChecked)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.InvoiceNumber, x => x.InvoiceNumber.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.ItemDescription, x => x.ItemDescription.Text)
                .DisposeWith(disposable);
        });
    }
}