using System.Reactive.Disposables;
using MyTime.Controller;
using ReactiveUI;
using ReactiveUI.Maui;

namespace MyTime.View;

public partial class AddEmployerPage : ReactiveContentPage<AddEmployerController>
{
    public AddEmployerPage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.Bind(ViewModel, x => x.SaveEmployerCommand, x => x.SaveButton.Command)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.Employer.Name ?? "New Employer", x => x.HeaderLabel)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.Employer.Name, x => x.NameLabel.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.Employer.EmployerNb, x => x.EmployerNumberLabel.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.Employer.Salary, x => x.SalaryLabel.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.Employer.AddressLine1, x => x.AddressLine1Label.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.Employer.AddressLine1, x => x.AddressLine2Label.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.Employer.Description, x => x.DescriptionLabel.Text)
                .DisposeWith(disposable);
        });
    }

    //protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    //{
    //    base.OnNavigatingFrom(args);
    //    Title = String.Empty;
    //}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Title = "Add Employer";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Title = String.Empty;
    }
}