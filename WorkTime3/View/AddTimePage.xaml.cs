using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using MyTime.Controller;
using ReactiveUI;
using ReactiveUI.Maui;

namespace MyTime.View;

public partial class AddTimePage : ReactiveContentPage<AddTimeController>
{
    public AddTimePage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.Bind(ViewModel, x => x.SelectedEmployer, x => x.EmployerPicker.SelectedItem)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, x => x.SaveTimeCommand, x => x.SaveButton.Command)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, x => x.DeleteTimeCommand, x => x.DeleteButton.Command)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, x => x.UpdateSalaryCommand, x => x.UpdateSalaryBehavior.Command)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, x => x.LoadCommand, x => x.LoadBehavior)
                .DisposeWith(disposable);
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Title = String.Empty;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Title = String.Empty;
    }
}