namespace WorkTime3.View;

public partial class EmployerPage : ContentPage
{
    public EmployerPage()
    {
        InitializeComponent();
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        Title = "Employers";
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Title = String.Empty;
    }
}