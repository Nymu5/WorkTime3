using System.Reactive.Disposables;
using MyTime.Controller;
using ReactiveUI;
using ReactiveUI.Maui;

namespace MyTime.View;

public partial class TimePage : ReactiveContentPage<TimeController>
{
    public TimePage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, x => x.Times, x => x.TimesCollection.ItemsSource)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.SearchTerm, x => x.SearchBar.Text)
                .DisposeWith(disposable);
            this.OneWayBind(ViewModel, x => x.FilterWorkedAmount, x => x.StatsTimes.Text)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.FilterStart, x => x.FilterStart.Date)
                .DisposeWith(disposable);
            this.Bind(ViewModel, x => x.FilterEnd, x => x.FilterEnd.Date)
                .DisposeWith(disposable);
        });
    }
}