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
            this.OneWayBind(ViewModel, x => x.Employers, x => x.EmployerCollection.ItemsSource)
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