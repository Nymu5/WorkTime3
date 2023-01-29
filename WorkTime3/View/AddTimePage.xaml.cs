using System.Reactive.Disposables;
using ReactiveUI;

namespace MyTime.View;

public partial class AddTimePage
{
    public AddTimePage()
    {
        InitializeComponent();

        this.WhenActivated(disposable =>
        {
            this.OneWayBind(ViewModel, x => x.Employers, x => x.EmployerPicker.ItemsSource)
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