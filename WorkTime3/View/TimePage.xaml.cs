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
            this.Bind(ViewModel, x => x.SearchTerm, x => x.SearchTermEditor.Text)
                .DisposeWith(disposable);
        });
    }
}