using System.Reactive.Disposables;
using ReactiveUI;

namespace MyTime.View;

public partial class EmployerPage
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
}