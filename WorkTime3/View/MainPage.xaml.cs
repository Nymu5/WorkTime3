using System.Reactive.Disposables;
using MyTime.Controller;
using ReactiveUI;
using ReactiveUI.Maui;

namespace MyTime.View;

public partial class MainPage : ReactiveContentPage<MainController>
{
    public MainPage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, x => x.StatsEmployers, x => x.StatsEmployers.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.StatsHours, x => x.StatsHours.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.StatsTotal, x => x.StatsTotal.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.StatsTimes, x => x.StatsTimes.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.Series, x => x.Chart.Series)
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