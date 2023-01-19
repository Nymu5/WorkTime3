using System.Reactive.Disposables;
using MyTime.Controller;
using ReactiveUI;
using ReactiveUI.Maui;

namespace MyTime.View;

public partial class EmployerPage : ReactiveContentPage<EmployerController>
{
    public EmployerPage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.Bind(ViewModel, x => x.Employers, x => x.EmployerCollection.ItemsSource)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.SelectedEmployer, x => x.EmployerCollection.SelectedItem)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, x => x.SelectionChangedCommand,
                    x => x.EmployerCollection.SelectionChangedCommand)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, x => x.CreateEmployer, x => x.CreateEmployerButton.Command)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.IsRefreshing, x => x.RefreshView.IsRefreshing)
                .DisposeWith(disposable);
            this.BindCommand(ViewModel, x => x.RefreshCommand, x => x.RefreshView.Command)
                .DisposeWith(disposable);
        });
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        //Title = "Employers";
        Title = String.Empty;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Title = String.Empty;
    }

    void RefreshView_Refreshing(System.Object sender, System.EventArgs e)
    {
    }
}