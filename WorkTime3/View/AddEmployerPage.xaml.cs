namespace MyTime.View;

public partial class AddEmployerPage : ContentPage
{
    public AddEmployerPage()
    {
        InitializeComponent();
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