namespace MyTime.View;

public partial class EmployerPage : ContentPage
{
    public EmployerPage()
    {
        InitializeComponent();
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